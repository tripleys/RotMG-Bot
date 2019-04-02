using System;
using System.Collections.Generic;
using System.Text;

namespace RotMG_Bot.Events
{
    public class RequestTradeEvent
    {
        public string Name;
        public int Timeout;
        public int Start;

        public Action<TradeEvent> OnAccept;
        public Action OnTimeout;

        public RequestTradeEvent Accepted(Action<TradeEvent> onAccept)
        {
            OnAccept = onAccept;
            return this;
        }

        public RequestTradeEvent TimedOut(Action onTimeout)
        {
            OnTimeout = onTimeout;
            return this;
        }
    }
}
