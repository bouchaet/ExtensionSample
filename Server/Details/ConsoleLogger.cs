using System;
using Entities;

namespace Server.Details
{
    internal class ConsoleLogger
    {
        static ConsoleLogger() => Subscribe();

        private static void Subscribe()
        {
            Logger.Singleton.Info += (source, msg) => WriteInfo(msg);
            Logger.Singleton.Warning += (source, msg) => WriteWarning(msg);

        }

        private static void WriteInfo(string s) => Write($"INFO {s}");

        private static void WriteWarning(string s) => Write($"WARNING {s}");

        private static void Write(string s) =>
            Console.WriteLine($"{DateTime.Now} {s}");

        public static void Init() => new ConsoleLogger();
    }

    internal class ConsoleDebug
    {
        static ConsoleDebug() => Debug.Singleton.Log += (source, msg) =>
                                   Console.WriteLine($"DEBUG: {msg}");

        public static void Init() => new ConsoleDebug();
    }
}
