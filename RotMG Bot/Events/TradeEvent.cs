using RotMG_Bot.Core;
using RotMG_Net_Lib.Models;
using RotMG_Net_Lib.Networking.Packets.Outgoing;
using System;
using System.Collections.Generic;
using System.Text;

namespace RotMG_Bot.Events
{
    public class TradeEvent
    {

        private Client _client;

        public bool[] ClientOffer = new bool[12];

        public bool[] PartnerOffer = new bool[12];

        public TradeItem[] ClientItems;
        public TradeItem[] PartnerItems;

        public TradeEvent(Client client, TradeItem[] clientItems, TradeItem[] partnerItems)
        {
            _client = client;
            ClientItems = clientItems;
            PartnerItems = partnerItems;
        }

        public Action OnCancel;
        public Action OnSuccess;
        public Action<List<int>> OnOffered;

        public bool HasItems(params int[] items)
        {
            for(byte i = 0; i < items.Length; i += 2) {
                byte found = 0;
                for (byte j = 4; j < PartnerItems.Length; j++)
                    if (PartnerItems[j].Item == items[i])
                    {
                        found++;
                    }
                if (found < items[i + 1]) return false;
            }
            return true;
        }

        public TradeEvent IfCanceled(Action onCancel)
        {
            OnCancel = onCancel;
            return this;
        }

        public TradeEvent Success(Action onSuccess)
        {
            OnSuccess = onSuccess;
            return this;
        }

        private void ResetClientOffer()
        {
            for (byte i = 0; i < ClientOffer.Length; i++)
                ClientOffer[i] = false;
        }

        public TradeEvent Select(params bool[] offer)
        {
            ResetClientOffer();
            for (byte i = 0; i < offer.Length; i++)
                ClientOffer[i + 4] = offer[i];
            _client.SendPacket(new ChangeTradePacket
            {
                Offer = ClientOffer
            });
            return this;
        }

        /// <summary>
        /// Select slots 1-8
        /// </summary>
        /// <param name="slots"></param>
        /// <returns></returns>
        public TradeEvent Select(params byte[] slots)
        {
            ResetClientOffer();
            foreach (byte slot in slots)
            {
                ClientOffer[3 + slot] = true;
            }
            _client.SendPacket(new ChangeTradePacket
            {
                Offer = ClientOffer
            });
            return this;
        }

        public TradeEvent Offered(Action<List<int>> onOffered)
        {
            OnOffered = onOffered;
            return this;
        }

        public void Accept()
        {
            _client.SendPacket(new AcceptTradePacket
            {
                ClientOffer = ClientOffer,
                PartnerOffer = PartnerOffer
            });
        }

        public void Cancel()
        {
            _client.SendPacket(new CancelTradePacket());
        }
    }
}
