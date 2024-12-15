using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2024.Utilities
{
    public static class NumberUtilities
    {
        public static int GreatestCommonDivisor(int a, int b)
        {
            // Swap to ensure a is the biggest number
            if (a < b)
            {
                int temp = a;
                a = b;
                b = temp;
            }

            // Remove
            while (b != 0)
            {
                // Rest between a and b
                int r = a % b;

                // We forget the biggest number, and prepare next iteration
                // by using b as a, and r as b.
                a = b;
                b = r;
            }

            return a;
        }

        public static int GreatestCommonDivisor(int[] integers)
        {
            // GCD of just one number is itself
            if (integers.Length == 1) return integers[0];

            int gcd = GreatestCommonDivisor(integers[0], integers[1]);
            for (int i = 2; i < integers.Length - 1; i++)
            {
                gcd = GreatestCommonDivisor(gcd, integers[i]);
            }
            return gcd;
        }

        public static int LeastCommonMultiple(int a, int b)
        {
            return Math.Abs(a * b) / GreatestCommonDivisor(a, b);
        }

        public static int LeastCommonMultiple(int[] integers)
        {
            // LCM of just one number is itself
            if (integers.Length == 1) return integers[0];

            int lcm = LeastCommonMultiple(integers[0], integers[1]);
            for (int i = 2; i < integers.Length - 1; i++)
            {
                lcm = NumberUtilities.LeastCommonMultiple(lcm, integers[i]);
            }
            return lcm;
        }

        public static long GreatestCommonDivisorLong(long a, long b)
        {
            // Swap to ensure a is the biggest number
            if (a < b)
            {
                long temp = a;
                a = b;
                b = temp;
            }

            // Remove
            while (b != 0)
            {
                // Rest between a and b
                long r = a % b;

                // We forget the biggest number, and prepare next iteration
                // by using b as a, and r as b.
                a = b;
                b = r;
            }

            return a;
        }

        public static long GreatestCommonDivisorLong(long[] integers)
        {
            // GCD of just one number is itself
            if (integers.Length == 1) return integers[0];

            long gcd = GreatestCommonDivisorLong(integers[0], integers[1]);
            for (int i = 2; i < integers.Length - 1; i++)
            {
                gcd = GreatestCommonDivisorLong(gcd, integers[i]);
            }
            return gcd;
        }

        public static long LeastCommonMultipleLong(long a, long b)
        {
            return Math.Abs(a * b) / GreatestCommonDivisorLong(a, b);
        }

        public static long LeastCommonMultipleLong(long[] integers)
        {
            // LCM of just one number is itself
            if (integers.Length == 1) return integers[0];

            long lcm = LeastCommonMultipleLong(integers[0], integers[1]);
            for (int i = 2; i < integers.Length - 1; i++)
            {
                lcm = NumberUtilities.LeastCommonMultipleLong(lcm, integers[i]);
            }
            return lcm;
        }

    }
}
