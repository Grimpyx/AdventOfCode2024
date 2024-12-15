using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2024.Utilities
{
    public struct Int2
    {
        public int x;
        public int y;

        public Int2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static Int2 operator +(Int2 a, Int2 b) => new Int2(a.x + b.x, a.y + b.y);
        public static Int2 operator -(Int2 a, Int2 b) => new Int2(a.x - b.x, a.y - b.y);
        public static Int2 operator *(int a, Int2 b) => new Int2(a * b.x, a * b.y);
        public static bool operator ==(Int2 a, Int2 b) => a.x == b.x && a.y == b.y;
        public static bool operator !=(Int2 a, Int2 b) => !(a == b);

        public readonly double Magnitude => Math.Sqrt((x * x) + (y * y));
        public static double Distance(Int2 a, Int2 b) => (b - a).Magnitude;
        public double DistanceTo(Int2 other) => (this - other).Magnitude;

        public readonly bool ContainsNegativeNumber => (x < 0) || (y < 0);

        public Int2 GetRotated90Clockwise(int times = 1)
        {
            if (times % 4 == 1) return new Int2(-y, x);
            if (times % 4 == 2) return new Int2(-x, -y);
            if (times % 4 == 3) return new Int2(y, -x);
            else return this;
        }

        public static Int2 Zero => new Int2(0, 0);
        public static Int2 One => new Int2(1, 1);
        public static Int2 Up => new Int2(0, -1);
        public static Int2 Right => new Int2(1, 0);
        public static Int2 Down => new Int2(0, 1);
        public static Int2 Left => new Int2(-1, 0);

        public override string? ToString()
        {
            return $"({x},{y})";
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (obj.GetHashCode() == GetHashCode()) return true;

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(x, y);
        }
    }
}
