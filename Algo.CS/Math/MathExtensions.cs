using System;
using System.Linq;

namespace Algo.CS.Math
{
    public static class IntegerSequence
    {
        public static long Fibon(int n)
        {
            if (n < 0) throw new ArgumentOutOfRangeException(nameof(n));

            if (n < 2) return n;
            if (n == 2) return 1;

            var f = new[] { 1L, 1L };
            try
            {
                for (var i = 3; i < n; i++)
                {
                    f[i % 2] = f.Sum();
                }
            }
            catch (ArithmeticException e)
            {
                Console.WriteLine($"Too large f1: {f[0]}, f2: {f[1]}. Exception : {e}");
            }

            //Console.WriteLine($"Fibon of {n} is {f.Sum()}");
            return f.Sum();
        }
    }
}
