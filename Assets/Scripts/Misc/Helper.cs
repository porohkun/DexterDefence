using Game;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class Helper
{
    public static bool IsOdd(this int num)
    {
        return num % 2 != 0;
    }

    public static Transform FindChildRecursively(this Transform parent, string name)
    {
        int count = parent.childCount;
        for (int i = 0; i < count; i++)
        {
            var child = parent.GetChild(i);
            if (child.name == name)
                return child;
            else
            {
                var result = child.FindChildRecursively(name);
                if (result != null)
                    return result;
            }
        }
        return null;
    }

    public static bool HaveFlag(this Direction direction, Direction flag)
    {
        return (direction & flag) == flag;
    }

    public static Direction AddFlag(this Direction direction, Direction flag)
    {
        return direction | flag;
    }

    public static Direction RemoveFlag(this Direction direction, Direction flag)
    {
        return direction & ~flag;
    }

    public static IEnumerable<Direction> Enumerate(this Direction direction)
    {
        if (direction.HaveFlag(Direction.North))
            yield return Direction.North;
        if (direction.HaveFlag(Direction.South))
            yield return Direction.South;
        if (direction.HaveFlag(Direction.West))
            yield return Direction.West;
        if (direction.HaveFlag(Direction.East))
            yield return Direction.East;
    }

    public static string Stringify(this Direction direction)
    {
        var sb = new StringBuilder();
        if (direction.HaveFlag(Direction.North))
            sb.Append('n');
        if (direction.HaveFlag(Direction.South))
            sb.Append('s');
        if (direction.HaveFlag(Direction.West))
            sb.Append('w');
        if (direction.HaveFlag(Direction.East))
            sb.Append('e');
        return sb.ToString();
    }

    public static Direction Mirror(this Direction direction)
    {
        Direction result = Direction.None;
        if (direction.HaveFlag(Direction.North))
            result.AddFlag(Direction.South);
        if (direction.HaveFlag(Direction.South))
            result.AddFlag(Direction.North);
        if (direction.HaveFlag(Direction.West))
            result.AddFlag(Direction.East);
        if (direction.HaveFlag(Direction.East))
            result.AddFlag(Direction.West);
        return result;
    }

    public static Direction ToDirection(this string text)
    {
        Direction result = Direction.None;
        foreach (var c in text)
            switch (c)
            {
                case 'n': result = result.AddFlag(Direction.North); break;
                case 's': result = result.AddFlag(Direction.South); break;
                case 'w': result = result.AddFlag(Direction.West); break;
                case 'e': result = result.AddFlag(Direction.East); break;
                default: throw new ArgumentException("cant parse this text to Direction");
            }
        return result;
    }
}
