using System;
using System.Collections.Generic;
using System.Text;

namespace RotMG_Bot.Plugins
{
    public class PluginManager
    {
        public static Dictionary<string, IPlugin> Plugins = new Dictionary<string, IPlugin>();

        private static void Install(IPlugin plugin)
        {
            Plugins.Add(plugin.Name(), plugin);
        }

        public static void Load()
        {
            //Install(new Test());
            Install(new PlayerTracker());
        }
    }
}
