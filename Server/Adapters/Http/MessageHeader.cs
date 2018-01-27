using System;

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


        public static MessageHeader From(string field, string value)
        {
            if(field.Contains(":"))
                throw new ArgumentException(nameof(field));
            if(value.Contains(":"))
                throw new ArgumentException(nameof(value));

            return new MessageHeader
            {
                Key = field,
                Value = value
            };
        }
    }
}