using System;

namespace Game
{
    [Flags]
    public enum Direction
    {
        None = 0,
        North = 1,
        South = 2,
        West = 4,
        East = 8
    }
}

