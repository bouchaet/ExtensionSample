namespace Server.Adapters.Http
{
    public class MessageHeader
    {
        public string Key { get; private set; }
        public string Value { get; private set; }

    public static MessageHeader From(string line)
        {
            var kv = line.Split(':');

            return new MessageHeader
            {
                Key = kv[0],
                Value = kv[1] ?? string.Empty
            };
        }
    }
}