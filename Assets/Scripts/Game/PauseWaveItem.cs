using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public class PauseWaveItem : IWaveItem
    {
        public float Interval { get; private set; }

        public PauseWaveItem(float interval)
        {
            Interval = interval;
        }
    }
}
