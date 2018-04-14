using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game;
using MimiJson;
using UnityEngine;

namespace Game
{
    public struct Point
    {
        public int X;
        public int Y;

        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public Point(JsonValue json) : this()
        {
            this.X = json["x"];
            this.Y = json["y"];
        }

        #region methods

        public static double Distance(Point P1, Point P2)
        {
            return P1.DistanceTo(P2);
        }

        public double DistanceTo(Point P)
        {
            return Math.Sqrt((this.X - P.X) * (this.X - P.X) + (this.Y - P.Y) * (this.Y - P.Y));
        }

        #endregion

        #region operatos

        public static bool operator ==(Point p1, Point p2)
        {
            return p1.X == p2.X && p1.Y == p2.Y;
        }

        public static bool operator !=(Point p1, Point p2)
        {
            return p1.X != p2.X || p1.Y != p2.Y;
        }

        public static Point operator +(Point p1, Point p2)
        {
            return new Point(p1.X + p2.X, p1.Y + p2.Y);
        }

        internal Point AddDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                    return new Point(X, Y + 1);
                case Direction.South:
                    return new Point(X, Y - 1);
                case Direction.West:
                    return new Point(X - 1, Y);
                case Direction.East:
                    return new Point(X + 1, Y);
                default:
                    return this;
            }
        }

        public static Point operator -(Point p1, Point p2)
        {
            return new Point(p1.X - p2.X, p1.Y - p2.Y);
        }

        public static Point operator /(Point p, float mod)
        {
            return new Point(Mathf.RoundToInt(p.X / mod), Mathf.RoundToInt(p.Y / mod));
        }

        public static Point operator *(Point p, float mod)
        {
            return new Point(Mathf.RoundToInt(p.X * mod), Mathf.RoundToInt(p.Y * mod));
        }

        #endregion

        public static implicit operator Point(Vector2 vec)
        {
            return new Point(Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y));
        }

        internal Point RectBorder(int max)
        {
            return new Point(
                X > max ? max : (X < -max ? -max : X),
                Y > max ? max : (Y < -max ? -max : Y)
                );
        }

        public static implicit operator Point(Vector3 vec)
        {
            return new Point(Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y));
        }

        public static implicit operator Vector2(Point p)
        {
            return new Vector2(p.X, p.Y);
        }

        public static implicit operator Vector3(Point p)
        {
            return new Vector3(p.X, p.Y);
        }

        #region Object

        public override bool Equals(object Point)
        {
            return this == (Point)Point;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() + Y.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("[{0}; {1}]", X, Y);
        }

        #endregion

        internal JsonValue ToJson()
        {
            return new JsonObject(new JOPair("x", X), new JOPair("y", Y));
        }

    }
}
