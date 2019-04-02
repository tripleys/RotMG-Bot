using RotMG_Bot.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace RotMG_Bot.Events
{
    public class MoveEvent
    {
        public Action<float, float> OnReach;

        public float X;
        public float Y;

        public void Reach(Action<float, float> reach)
        {
            OnReach = reach;
        }
    }
}
