using RotMG_Bot.Core;
using RotMG_Bot.Plugins;
using RotMG_Net_Lib.Networking.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace RotMG_Bot
{
    class PlayerTracker : IPlugin
    {
        public void Connect(Client client)
        {
        }

        public void Disconnect(Client client)
        {
            //throw new NotImplementedException();
        }

        public void Instance(Client client)
        {
            client.Track().Enter((p) => client.Log($"{p.Name} Entered.")).Leave((p) => client.Log($"{p.Name} Left."));
        }

        public string Name() => "PlayerTracker";
    }
}
