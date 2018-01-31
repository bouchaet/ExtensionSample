using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Algo.CS.Math.IntegerSequence;

namespace ParaPerf
{
    static class Program
    {
        static void Main(string[] args)
        {
            const int repeat = 100;
            do
            {
                try
                {
                    Console.Write("Choose io, cpu-fibo, cpu-mining: ");
                    var actions = SelectFactoryFunc(Console.ReadLine())
                        .Invoke(repeat)
                        .ToArray();

                    actions.DoSynchonously();
                    actions.DoAsynchronously();
                    actions.DoParallel();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Oupsy... {e}");
                }

                Console.WriteLine("Again? (y,[n])");
            } while (Console.ReadLine() == "y");

            Console.Write("fibo(x) x=>");
            Console.Write(Fibon(int.Parse(Console.ReadLine())));
            Console.ReadLine();
        }

        private static void DoSynchonously(this Func<string>[] actions)
        {
            actions.Measure(
                x => x.Select(a => a.Invoke()).ToArray(),
                "Sync");
        }

        private static void DoAsynchronously(this Func<string>[] actions)
        {
            actions.Measure(
                x =>
                {
                    var taskFactory = new TaskFactory();
                    var tasks = x.Select(a => taskFactory.StartNew(a.Invoke)).ToList();
                    Task.WaitAll(tasks.Cast<Task>().ToArray());
                    return tasks.Select(t => t.Result).ToArray();
                },
                "Async/Await"
            );
        }

        private static void DoParallel(this Func<string>[] actions)
        {
            actions.Measure(
                x => (from a in x.AsParallel() select a.Invoke()).ToArray(),
                "PLINQ");

            //    actions.Measure(
            //        x => Parallel.ForEach(x, func => func.Invoke()).,"PLINQ ForEach");
        }

        private static void Measure(
            this Func<string>[] actions,
            Func<Func<string>[], string[]> loop,
            string name)
        {
            var sw = new Stopwatch();
            sw.Start();
            var results = loop.Invoke(actions);
            sw.Stop();
            Console.WriteLine(
                $"{name} took {sw.ElapsedMilliseconds}ms. Last result : \"{results.Last()}\"");
        }

        private static Func<int, IEnumerable<Func<string>>> SelectFactoryFunc(string s)
        {
            var hashInput = Guid.NewGuid().ToString();

            if (s == "io")
                return n => Enumerable.Repeat<Func<string>>(
                    () =>
                    {
                        Thread.Sleep(100);
                        return string.Empty;
                    },
                    n);

            if (s == "cpu-mining")
                return n => Enumerable.Repeat<Func<string>>(
                    () =>
                    {
                        var (hash, seed) = Mine(hashInput);
                        return $"{hashInput} mine to {hash} with seed {seed}.";
                    },
                    n);

            if (s == "cpu-fibo")
                return n => Enumerable.Repeat<Func<string>>(
                    () =>
                    {
                        const int x = 60;
                        return $"fibonnaci({x}) = {Fibon(x)}";
                    },
                    n * n);

            throw new ArgumentException(nameof(s));
        }

        private static (string Hash, int Seed) Mine(string text)
        {
            var md5 = MD5.Create();
            var hash = text;
            var seed = -1;
            while (!hash.StartsWith("0000"))
            {
                var input = $"{text}-{++seed}";
                hash = GetMd5Hash(md5, input);
            }
            return (hash, seed);
        }

        private static string GetMd5Hash(HashAlgorithm algo, string input)
        {
            var data = algo.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sBuilder = new StringBuilder();
            foreach (var t in data)
            {
                sBuilder.Append(t.ToString("x2"));
            }

            return sBuilder.ToString();
        }
    }
}