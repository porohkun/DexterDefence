using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public class UnitWaveItem : IWaveItem
    {
        public string Name { get; private set; }
        public int Count { get; private set; }
        public float Interval { get; private set; }

        public UnitWaveItem(string name, int count, float interval)
        {
            Name = name;
            Count = count;
            Interval = interval;
        }
    }
}
