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
        //public static Int2 operator /(Int2 a, int b) => new Int2(a.x /, a.y);

        public readonly double Magnitude => Math.Sqrt((x * x) + (y * y));
        public static double Distance(Int2 a, Int2 b) => (b - a).Magnitude;
        public double DistanceTo(Int2 other) => (this - other).Magnitude;
    }
}
