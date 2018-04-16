using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public class SingleShotAI : ITowerAI
    {
        public float Radius { get; private set; }

        public SingleShotAI(float radius)
        {
            Radius = radius;
        }
    }
}
