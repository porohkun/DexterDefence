using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public class Wave
    {
        public List<IWaveItem> Items { get; private set; }

        public Wave()
        {
            Items = new List<IWaveItem>();
        }
    }
}
