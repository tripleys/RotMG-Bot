using RotMG_Bot.Data;
using RotMG_Bot.Events;
using RotMG_Bot.Plugins;
using RotMG_Net_Lib.Crypto;
using RotMG_Net_Lib.Data;
using RotMG_Net_Lib.Models;
using RotMG_Net_Lib.Networking;
using RotMG_Net_Lib.Networking.Packets;
using RotMG_Net_Lib.Networking.Packets.Incoming;
using RotMG_Net_Lib.Networking.Packets.Outgoing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace RotMG_Bot.Core
{
    public class Client
    {
        public const float MinMoveSpeed = 0.004f;
        public const float MaxMoveSpeed = 0.0096f;

        public static string BuildVersion = "?";

        private NetClient _net;

        private Account _account;

        private List<IPlugin> _plugins;

        private Queue<MoveEvent> _moveQueue;

        private Stopwatch _watch;

        private int _lastTickTime;
        private int _currentTickTime;

        private List<MoveRecord> _records;

        private RequestTradeEvent _request;

        private TradeEvent _trade;

        private TrackEvent _tracking;

        public Reconnect Reconnect;
        public string NexusIp;

        public int CharId;

        public int ObjectId;

        public WorldPosData position;

        public PlayerData PlayerData;

        public Dictionary<int, PlayerData> Players;

        public Client(Account account, List<IPlugin> plugins)
        {
            Players = new Dictionary<int, PlayerData>();
            _moveQueue = new Queue<MoveEvent>();
            _net = new NetClient();
            _account = account;
            _records = new List<MoveRecord>();
            ApplyHooks(); // apply client hooks before plugin hooks
            (_plugins = plugins).ForEach(p => p.Instance(this));
            _net.AddConnectionListener(Connected);
            _net.AddDisconnectListener(Disconnected);
            _watch = new Stopwatch();
            _net.Connect(Reconnect = Servers.NameToRecon(account.Server));
            NexusIp = Reconnect.Host;
        }

        private void ApplyHooks()
        {
            _net.Hook(PacketType.Failure, p => OnFailure((FailurePacket)p));
            _net.Hook(PacketType.MapInfo, p => OnMapInfo((MapInfoPacket)p));
            _net.Hook(PacketType.CreateSuccess, p => OnCreateSuccess((CreateSuccessPacket)p));
            _net.Hook(PacketType.Update, p => OnUpdate((UpdatePacket)p));
            _net.Hook(PacketType.NewTick, p => OnNewTick((NewTickPacket)p));
            _net.Hook(PacketType.Goto, p => OnGoto((GotoPacket)p));
            _net.Hook(PacketType.Ping, p => OnPing((PingPacket)p));
            _net.Hook(PacketType.TradeStart, p => OnTradeStart((TradeStartPacket)p));
            _net.Hook(PacketType.TradeRequested, p => OnTradeRequested((TradeRequestedPacket)p));
            _net.Hook(PacketType.TradeDone, p => OnTradeDone((TradeDonePacket)p));
            _net.Hook(PacketType.TradeChanged, p => OnTradeChanged((TradeChangedPacket)p));
            _net.Hook(PacketType.Reconnect, p => OnReconnect((ReconnectPacket)p));
        }

        public void Hook(PacketType type, Action<Client, IncomingPacket> action)
        {
            _net.Hook(type, p => action(this, p));
        }

        private void Reset()
        {
            Players.Clear();
            _request = null;
            _trade = null;
            _lastTickTime = 0;
            _currentTickTime = 0;
            _watch.Restart();
            _moveQueue.Clear();
        }

        private void Connected()
        {
            Reset();
            HelloPacket hello = new HelloPacket
            {
                BuildVersion = BuildVersion,
                GameId = Reconnect.GameId,
                Guid = RSA.Instance.Encrypt(_account.Email),
                Password = RSA.Instance.Encrypt(_account.Password),
                Secret = RSA.Instance.Encrypt(""),
                Key = Reconnect.Key,
                KeyTime = Reconnect.KeyTime,
                GameNet = "rotmg",
                PlayPlatform = "rotmg"
            };
            _net.SendPacket(hello);
        }

        private void Disconnected()
        {
            Log("Disconnected");
            _plugins.ForEach(p => p.Disconnect(this));
            Log($"Reconnecting in {Program.Config.ReconnectDelay / 1000}");
            Task.Delay(Program.Config.ReconnectDelay).ContinueWith((t) =>
            {
                _net.Connect(Reconnect);
            });
        }

        public void Nexus()
        {
            Reconnect.Host = NexusIp;
            Reconnect.GameId = (int)GameIds.Nexus;
            _net.Disconnect();
        }

        public void Vault()
        {
            Reconnect.Host = NexusIp;
            Reconnect.GameId = (int)GameIds.Vault;
            _net.Disconnect();
        }

        public void Log(string message)
        {
            Console.WriteLine($"[{_account.Email}]: {message}");
        }

        private void OnFailure(FailurePacket failure)
        {
            Log($"Failure: {failure.ErrorDescription}");
        }

        private void OnMapInfo(MapInfoPacket mapInfo)
        {
            Log($"Connecting to {mapInfo.Name}");
            if (_account.CharId > 0)
            {
                _net.SendPacket(new LoadPacket
                {
                    CharId = _account.CharId,
                    IsFromArena = false
                });
            }
            else
            {
                _net.SendPacket(new CreatePacket
                {
                    ClassType = 782,
                    SkinType = 0
                });
            }
        }

        private void OnReconnect(ReconnectPacket reconnect)
        {
            if (reconnect.Host.Length > 0)
                Reconnect.Host = reconnect.Host;
            Reconnect.GameId = reconnect.GameId;
            Reconnect.Key = reconnect.Key;
            Reconnect.KeyTime = reconnect.KeyTime;
            Log($"{reconnect.Key}|{reconnect.Key.Length} + {reconnect.KeyTime} + {reconnect.GameId} + {reconnect.Host}");
            _net.Disconnect();
        }

        private void OnCreateSuccess(CreateSuccessPacket createSuccess)
        {
            Log("Connected");
            CharId = createSuccess.CharId;
            ObjectId = createSuccess.ObjectId;
            _plugins.ForEach(p => p.Connect(this));
        }

        private void OnUpdate(UpdatePacket update)
        {
            _net.SendPacket(new UpdateAckPacket());
            foreach(var obj in update.NewObjects)
            {
                if (obj.Status.ObjectId == ObjectId)
                {
                    PlayerData = PlayerData.processObjectStatus(obj.Status);
                    position = obj.Status.Pos;
                }
                else if (Classes.NamesToTypes.ContainsValue(obj.ObjectType))
                {
                    if (Players.ContainsKey(obj.Status.ObjectId))
                    {
                        Players.Remove(obj.Status.ObjectId);
                    }
                    Players.Add(obj.Status.ObjectId, PlayerData.processObjectStatus(obj.Status));
                    if (_tracking != null)
                    {
                        _tracking.OnEnter?.Invoke(Players[obj.Status.ObjectId]);
                    }
                }
            }
            foreach(var drop in update.Drops)
            {
                if (Players.ContainsKey(drop))
                {
                    if (_tracking != null)
                    {
                        _tracking.OnLeave?.Invoke(Players[drop]);
                    }
                    Players.Remove(drop);
                }
            }
        }

        private void OnNewTick(NewTickPacket newTick)
        {
            _lastTickTime = _currentTickTime;
            int time = GetTime();
            _currentTickTime = time;
            if (_request != null && time - _request.Start > _request.Timeout)
            {
                _request.OnTimeout?.Invoke();
                _request = null;
            }
            if (_moveQueue.Count > 0)
            {
                MoveEvent e = _moveQueue.Peek();
                MoveTo(e.X, e.Y);
            }
            _net.SendPacket(new MovePacket
            {
                TickId = newTick.TickId,
                NewPosition = position,
                Time = GetTime(),
                Records = _records
            });
            foreach(var status in newTick.Statuses)
            {
                if (Players.ContainsKey(status.ObjectId))
                {
                    PlayerData.processStatData(status.Stats, Players[status.ObjectId]);
                }
            }
        }

        private void OnGoto(GotoPacket go)
        {
            _net.SendPacket(new GotoAckPacket
            {
                Time = GetTime()
            });
        }

        private void OnPing(PingPacket ping)
        {
            _net.SendPacket(new PongPacket
            {
                Serial = ping.Serial,
                Time = GetTime()
            });
        }

        private void OnTradeStart(TradeStartPacket tradeStart)
        {
            if (_request != null && tradeStart.PartnerName.Equals(_request.Name, StringComparison.CurrentCultureIgnoreCase))
            {
                Log($"Trade accepted by {tradeStart.PartnerName}");
                _request.OnAccept?.Invoke(_trade = new TradeEvent(this, tradeStart.ClientItems, tradeStart.PartnerItems));
                _request = null;
                return;
            }
            // wtf
            _net.SendPacket(new CancelTradePacket());
        }

        private void OnTradeRequested(TradeRequestedPacket tradeRequested)
        {
            Log($"Trade request from {tradeRequested.Name}");
        }

        private void OnTradeDone(TradeDonePacket tradeDone)
        {
            Log($"{tradeDone.Result} | {tradeDone.Description}");
            if (_trade != null)
            {
                if (tradeDone.Result == TradeResult.Successful)
                {
                    _trade.OnSuccess?.Invoke();
                } else
                {
                    _trade.OnCancel?.Invoke();
                }
                _trade = null;
            }
        }

        private void OnTradeChanged(TradeChangedPacket tradeChanged)
        {
            // WTF?
            if (_trade == null) return;
            List<int> offer = new List<int>();
            for(byte i = 4; i < tradeChanged.Offer.Length; i++)
            {
                if (tradeChanged.Offer[i])
                {
                    offer.Add(_trade.PartnerItems[i].Item);
                }
            }
            _trade.PartnerOffer = tradeChanged.Offer;
            _trade.OnOffered?.Invoke(offer);
        }

        private void OnTradeAccepted(TradeAcceptedPacket tradeAccepted)
        {
            if (_trade == null) return;
            List<int> offer = new List<int>();
            for(byte i = 4; i < tradeAccepted.PartnerOffer.Length; i++)
            {
                if (tradeAccepted.PartnerOffer[i])
                {
                    offer.Add(_trade.PartnerItems[i].Item);
                }
            }
            _trade.OnOffered?.Invoke(offer);
        }

        private void MoveTo(float x, float y)
        {
            float speed = GetSpeed();
            if (position.DistanceTo(x, y) > speed) {
                double angle = Math.Atan2(y - position.Y, x - position.X);
                position.X += (float)(speed * Math.Cos(angle));
                position.Y += (float)(speed * Math.Sin(angle));
            }
            else
            {
                position.X = x;
                position.Y = y;
                _moveQueue.Dequeue().OnReach?.Invoke(x, y);
            }
        }

        public int GetTime()
        {
            return (int)_watch.ElapsedMilliseconds;
        }

        public float GetSpeed()
        {
            float speed = MinMoveSpeed + PlayerData.Speed / 75 * (MaxMoveSpeed - MinMoveSpeed);
            float multiplier = 1;
            int tickTime = _currentTickTime - _lastTickTime;
            if (tickTime > 200)
                tickTime = 200;
            return (speed * multiplier * tickTime);
        }

        public void SendPacket(OutgoingPacket packet)
        {
            _net.SendPacket(packet);
        }

        public TrackEvent Track()
        {

            _tracking = new TrackEvent();

            return _tracking;
        }

        public MoveEvent Move(float x, float y)
        {
            MoveEvent e = new MoveEvent
            {
                X = x,
                Y = y
            };
            _moveQueue.Enqueue(e);
            return e;
        }
        
        public RequestTradeEvent RequestTrade(string name, int timeout = 30000)
        {
            _net.SendPacket(new RequestTradePacket {
                Name = name
            });
            return (_request = new RequestTradeEvent
            {
                Name = name,
                Timeout = timeout
            });
        }
    }
}
