using RotMG_Bot.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace RotMG_Bot.Events
{
    public class TrackEvent
    {
        public Action<PlayerData> OnEnter;
        public Action<PlayerData> OnLeave;

        public TrackEvent Enter(Action<PlayerData> onEnter)
        {
            OnEnter = onEnter;
            return this;
        }

        public TrackEvent Leave(Action<PlayerData> onLeave)
        {
            OnLeave = onLeave;
            return this;
        }
    }
}
