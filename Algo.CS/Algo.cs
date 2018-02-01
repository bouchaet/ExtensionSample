using System;
using System.Collections.Generic;
using System.Linq;

public static class Algo
{
    #region Sequence Search

    public static int NaiveSearch(IReadOnlyList<byte> source, IReadOnlyList<byte> pattern)
    {
        var n = source.Count - 1;
        var m = pattern.Count - 1;

        for (var i = 0; i <= n - m + 1; i++)
        {
            for (var j = 0; j <= m; j++)
            {
                if (source[i + j] != pattern[j])
                    break;
                if (j == m)
                    return i;
            }
        }
        return -1;
    }

    #endregion

    #region Integer Series

    public static long Fibon(int n)
    {
        if (n < 0) throw new ArgumentOutOfRangeException(nameof(n));

        if (n < 2) return n;
        if (n == 2) return 1;

        var f = new[] {1L, 1L};
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

    #endregion
}