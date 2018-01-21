namespace Entities
{
    public class Logger
    {
        public static Logger Singleton;

        static Logger() => Singleton = new Logger();

        private Logger() { }

        public event System.EventHandler<string> Info;

        public event System.EventHandler<string> Warning;

        public event System.EventHandler<string> Error;

        public static void WriteInfo(string s) =>
            Singleton.Info?.Invoke(Singleton, s);

        public static void WriteWarning(string s) =>
            Singleton.Warning?.Invoke(Singleton, s);

        public static void WriteError(string s) =>
            Singleton.Error?.Invoke(Singleton, s);
    }

    public class Debug
    {
        static Debug() => Singleton = new Debug();

        private Debug()
        {
        }

        public static Debug Singleton { get; }

        public event System.EventHandler<string> Log;

        public static void Write(string msg) =>
            Singleton.Log?.Invoke(Singleton, msg);
    }
}