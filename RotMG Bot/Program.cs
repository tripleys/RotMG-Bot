using Newtonsoft.Json;
using RotMG_Bot.Core;
using RotMG_Bot.Data;
using RotMG_Bot.Plugins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace RotMG_Bot
{
    class Program
    {

        public static RunConfig Config;
        public static List<Client> Clients = new List<Client>();

        static void Main(string[] args)
        {
            PluginManager.Load();
            Config = JsonConvert.DeserializeObject<RunConfig>(File.ReadAllText("../../../Config.json"));
            Client.BuildVersion = Config.BuildVersion;
            List<IPlugin> plugins = new List<IPlugin>();
            foreach (var p in Config.Plugins)
            {
                if (PluginManager.Plugins.ContainsKey(p))
                    plugins.Add(PluginManager.Plugins[p]);
            }
            foreach(var acc in Config.Accounts)
            {
                Task.Run(() => Clients.Add(new Client(acc, plugins)));
            }
            
            while(true)
            {
                string command = Console.ReadLine();
                Console.WriteLine("Command: " + command);
            }
        }

        public static void UpdateConfig()
        {
            File.WriteAllText("../../../Config.json", JsonConvert.SerializeObject(Config));
        }
    }
}
