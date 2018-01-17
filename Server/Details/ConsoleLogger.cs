using System;
using Entities;

namespace Server.Details
{
    internal class ConsoleLogger : ILogger
    {
        public void WriteInfo(string s)
        {
            Write($"INFO {s}");
        }

        public void WriteWarning(string s)
        {
            Write($"WARNING {s}");
        }

        private static void Write(string s)
        {
            var now = DateTime.Now;
            Console.WriteLine($"{now} {s}");
        }
    }
}
