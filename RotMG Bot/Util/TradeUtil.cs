using System;
using System.Collections.Generic;
using System.Text;

namespace RotMG_Bot.Util
{
    public static class TradeUtil
    {
        public static bool Is(this List<int> ids, params int[] items)
        {
            for (int i = 0; i < items.Length; i += 2)
                if (ids.FindAll(k => k == items[i]).Count < items[i + 1])
                    return false;
            return true;
        }
    }
}
