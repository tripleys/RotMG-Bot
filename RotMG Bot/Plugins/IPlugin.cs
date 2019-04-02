using RotMG_Bot.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace RotMG_Bot.Plugins
{
    public interface IPlugin
    {
        string Name();
        /// <summary>
        /// New Client instance was created. 
        /// Called only once per Client. 
        /// Initiate your custom packet hooks here.
        /// </summary>
        /// <param name="client"></param>
        void Instance(Client client);
        /// <summary>
        /// Client successfully connected to Game, 
        /// Emitted on Success Packet.
        /// </summary>
        /// <param name="client"></param>
        void Connect(Client client);
        /// <summary>
        /// Client disconnected from the Server.
        /// </summary>
        /// <param name="client"></param>
        void Disconnect(Client client);
    }
}
