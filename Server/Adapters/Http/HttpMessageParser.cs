using System;
using System.Linq;
using System.Text;
using Entities;

namespace Server.Adapters.Http
{
    internal static class HttpMessageParser
    {
        public static IHttpRequest Parse(byte[] bytes, int offset, int size)
        {
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset));
            if (offset + size > bytes.Length)
                throw new ArgumentOutOfRangeException(nameof(size));

            var message =
                Encoding.ASCII.GetString(
                    bytes,
                    offset,
                    size); // todo: decode only startline and headers
            var extract = message.Substring(0, Math.Min(message.Length, 500));
            Debug.Write(
                $"Parsing raw request (showing max 500 characters):" +
                $"\r\n{extract}");

            var lines = message.Split("\r\n");

            if (lines.Length == 0)
                return new HttpRequest();

            (HttpVerb verb, string uri, string version) = ParseRquestLine(lines[0]);
            var headers = lines.Skip(1)
                .TakeWhile(line => line.Length > 0)
                .Select(MessageHeader.From);

            // todo: body must be decoded for POST and PUT only and based on header Content-Type
            var body = lines
                .SkipWhile(line => line.Length > 0)
                .Aggregate((cur, next) => cur + next);

            var req = new HttpRequest()
            {
                Verb = verb,
                RequestUri = uri,
                HttpVersion = version,
                Headers = headers.ToArray(),
                Body = body
            };

            if (uri.Contains("?"))
            {
                var questionMarkPos = uri.IndexOf("?", StringComparison.Ordinal);
                if (uri.Substring(questionMarkPos).Length > 1)
                    req.AddQueryParameters(
                        uri.Split("?")[1].Split("&").Select(p => p.Split("="))
                            .Select(x => (x[0], x[1] ?? string.Empty))
                    );
            }

            return req;
        }

        private static (HttpVerb, string, string) ParseRquestLine(string s)
        {
            if (string.IsNullOrEmpty(s))
                throw new ArgumentException(nameof(s));

            var parts = s.Split(' ');

            return (
                (HttpVerb) Enum.Parse(typeof(HttpVerb), parts[0], true),
                parts.Length > 1 ? parts[1] : string.Empty,
                parts.Length > 2 ? parts[2] : string.Empty
                );
        }
    }
}