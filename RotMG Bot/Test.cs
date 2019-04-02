using RotMG_Bot.Core;
using RotMG_Bot.Plugins;
using RotMG_Bot.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace RotMG_Bot
{
    class Test : IPlugin
    {
        public void Connect(Client client)
        {
            client.Log("Connected, starting to move.");
            client.RequestTrade("testguy", 10000).Accepted((trade) => {

                if (!trade.HasItems(0xa9d, 1, 0xad6, 1, 0xadc, 1, 0xa4e, 1))
                {
                    trade.Cancel();
                    return;
                }
                
                trade.IfCanceled(() => client.Log("Trade canceled.."))
                .Select(true, true, true, true)
                .Offered((offer) => {
                    if (offer.Is(0xa9d, 1, 0xad6, 1, 0xadc, 1, 0xa4e, 1))
                    {
                        trade.Accept();
                    }
                }).Success(() => client.Log("Trade successful"));

            }).TimedOut(() => client.Log("Trade request timed out"));
        }

        public void Disconnect(Client client)
        { 
        }

        public void Instance(Client client)
        {
        }

        public string Name()
        {
            return "Test";
        }
    }
}
