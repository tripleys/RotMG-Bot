using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RotMG_Bot.Util
{
    public static class PluginUtils
    {
        public static void Delayed(int millis, Action call)
        {
            Task.Delay(millis).ContinueWith((t) => call());
        }
    }
}
