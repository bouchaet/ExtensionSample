using System;
using Entities;

namespace Server.Details
{
    internal class ConsoleLogger : ILogger
    {
        public void WriteInfo(string s)
        {
            var now = DateTime.Now;
            Console.WriteLine($"{now} {s}");
        }
    }
}
