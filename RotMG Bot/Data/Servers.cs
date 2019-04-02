using Newtonsoft.Json;
using RotMG_Net_Lib.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RotMG_Bot.Data
{
    public class ServerModel
    {
        public string Host;
        public int Port;

        public Reconnect Nexus()
        {
            return new Reconnect
            {
                Host = Host,
                Port = Port,
                GameId = -2,
                Key = new byte[0],
                KeyTime = 0
            };
        }
    }

    public class Servers
    {
        private static Servers servers = JsonConvert.DeserializeObject<Servers>(File.ReadAllText("../../../Servers.json"));

        public static Reconnect NameToRecon(string server)
        {
            if (!servers.ServerData.ContainsKey(server))
            {
                throw new Exception("Server not found: " + server);
            }
            return servers.ServerData[server].Nexus();
        }

        public Dictionary<string, ServerModel> ServerData;
    }
}
