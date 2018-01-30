using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entities;
using Entities.Http;

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

            // todo: decode only startline and headers (end-of-header)
            var emptyLinePosition = FindEmptyLine(bytes, offset, size);
            Debug.Write($"Empty line found at position {emptyLinePosition}");

            var message =
                Encoding.ASCII.GetString(bytes, offset, emptyLinePosition + 1);
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
            // var body = lines
            //     .SkipWhile(line => line.Length > 0)
            //     .Aggregate((cur, next) => cur + next);
            var bodyPosition = emptyLinePosition + 2; //skip CRLF - 2 bytes
            var bodySize = size - bodyPosition - 1;
            var body = bodySize > 0
                ? Encoding.ASCII.GetString(bytes, bodyPosition, bodySize)
                : string.Empty;
            Debug.Write($"Body position and size: ({bodyPosition}, {bodySize})");
            
            var req = new HttpRequest()
            {
                Verb = verb,
                RequestUri = uri,
                HttpVersion = version,
                MessageHeaders = headers.ToArray(),
                Body = body
            };
            req.AddPathParameters(ParseQueryParameters(uri));

            return req;
        }

        private static IEnumerable<(string Key, string Value)> ParseQueryParameters(string uri)
        {
            if (uri.Contains("?"))
            {
                var questionMarkPos = uri.IndexOf("?", StringComparison.Ordinal);
                return uri.Substring(questionMarkPos).Length > 1
                    ? uri.Split("?")[1].Split("&").Select(p => p.Split("="))
                            .Select(x => (x[0], x[1] ?? string.Empty))
                    : Enumerable.Empty<(string,string)>();
            }
            return Enumerable.Empty<(string,string)>();
        }

        private static int FindEmptyLine(byte[] bytes, int offset, int size)
        {
            var TwoCrLf = new byte[4] { 0x13, 0x10, 0x13, 0x10 };
            var movingBytes = new byte[4] { 0x00, 0x00, 0x00, 0x00 };
            byte[] tempArray = new byte[4];
            return bytes.Skip(offset).Take(size).TakeWhile(
                x =>
                {
                    // shift left
                    Array.Copy(movingBytes, 0, tempArray, 0, 4);
                    Array.Copy(tempArray, 1, movingBytes, 0, 3);
                    movingBytes[3] = x;

                    return !movingBytes.SequenceEqual(TwoCrLf);
                })
                .Count() - 1;
        }

        private static (HttpVerb, string, string) ParseRquestLine(string s)
        {
            if (string.IsNullOrEmpty(s))
                throw new ArgumentException(nameof(s));

            var parts = s.Split(' ');

            return (
                (HttpVerb)Enum.Parse(typeof(HttpVerb), parts[0], true),
                parts.Length > 1 ? parts[1] : string.Empty,
                parts.Length > 2 ? parts[2] : string.Empty
                );
        }
    }
}