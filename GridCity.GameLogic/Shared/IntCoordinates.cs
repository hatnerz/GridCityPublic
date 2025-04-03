using System;

namespace GridCity.GameLogic.Shared
{
    public struct IntCoordinates
    {
        public int X { get; set; }
        public int Y { get; set; }
        public IntCoordinates(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static bool operator ==(IntCoordinates left, IntCoordinates right)
        {
            return left.X == right.X && left.Y == right.Y;
        }

        public static bool operator !=(IntCoordinates left, IntCoordinates right)
        {
            return !(left == right);
        }

        public override bool Equals(object? obj)
        {
            if (obj is IntCoordinates other)
                return this == other;
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }
    }
}
