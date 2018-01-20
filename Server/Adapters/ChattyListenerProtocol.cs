using System;
using Entities;

namespace Server.Adapters
{
    internal class ChattyListenerProtocol : ListenerProtocol
    {
        protected override string Greetings =>
            $"Welcome. Enter '{Exit}' to terminate." +
            Environment.NewLine;

        protected override string Goodbye => "Bye!";

        public override string Accept => "accept";

        public override string EOF => Environment.NewLine;

        public override string Exit => "quit";

        protected override string GetNext(string request)
        {
            return string.Empty;
        }
    }
}