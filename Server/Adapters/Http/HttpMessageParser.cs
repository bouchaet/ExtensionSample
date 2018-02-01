using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

            var emptyLinePosition = FindEmptyLine(bytes, offset, size);
            var headerBlock = Encoding.ASCII.GetString(bytes, offset, emptyLinePosition);
            var lines = headerBlock.Split("\r\n");

            LogFirstLines(headerBlock, emptyLinePosition);

            return lines.Any()
                ? BuildRequest(bytes, size, emptyLinePosition, lines)
                : new HttpRequest(); // is that enough?
        }

        private static HttpRequest BuildRequest(
            byte[] bytes,
            int size,
            int emptyLinePosition,
            IReadOnlyList<string> lines)
        {
            (HttpVerb verb, string uri, string version) = ParseRequestLine(lines[0]);
            var headers = ParseHeaders(lines.Skip(1));
            var body = ParseBody(bytes, size, emptyLinePosition);

            var req = new HttpRequest()
            {
                Verb = verb,
                RequestUri = uri,
                HttpVersion = version,
                MessageHeaders = headers,
                Body = body
            };
            req.AddQueryParameters(ParseQueryParameters(uri));
            return req;
        }

        private static void LogFirstLines(string firstLines, int emptyLinePosition)
        {
            Debug.Write($"Empty line found at position {emptyLinePosition}");

            var extract = firstLines.Substring(0, Math.Min(firstLines.Length, 500));
            Debug.Write(
                $"Parsing raw request (showing max 500 characters):" +
                $"\r\n{extract}");
        }

        private static IEnumerable<MessageHeader> ParseHeaders(IEnumerable<string> lines)
        {
            return lines
                .TakeWhile(line => line.Length > 0)
                .Select(MessageHeader.From);
        }

        private static string ParseBody(byte[] bytes, int size, int emptyLinePosition)
        {
            // todo: body must be decoded for POST and PUT only and based on header Content-Type
            // var body = lines
            //     .SkipWhile(line => line.Length > 0)
            //     .Aggregate((cur, next) => cur + next);
            var bodyPosition = emptyLinePosition + 4; //skip CR LF CR LF so 4 bytes
            var bodySize = size - bodyPosition;
            var body = bodySize > 0
                ? Encoding.ASCII.GetString(bytes, bodyPosition, bodySize)
                : string.Empty;
            Debug.Write($"Body position and size: ({bodyPosition}, {bodySize})");
            Debug.Write($"Body: [[[!{body}!]]]");
            return body;
        }

        private static IEnumerable<(string Key, string Value)> ParseQueryParameters(string uri)
        {
            var questionMarkPos = uri.IndexOf("?", StringComparison.Ordinal);
            return questionMarkPos != -1 && questionMarkPos < uri.Length - 1
                ? uri.Split("?")[1]
                    .Split("&")
                    .Select(p => p.Split("="))
                    .Select(x => (x[0], x.Length > 1 ? x[1] : string.Empty))
                : new(string, string)[] { };
        }

        private static int FindEmptyLine(byte[] bytes, int offset, int size)
        {
            // todo: implement Rabin-Karp with Rabin fingerprint instead
            //var TwoCrLf = new byte[4] {0b00001101, 0b00001010, 0b00001101, 0b00001010};
            //var movingBytes = new byte[4] { 0x00, 0x00, 0x00, 0x00 };
            //byte[] tempArray = new byte[4];
            //return bytes.Skip(offset).Take(size).TakeWhile(
            //    x =>
            //    {
            //        // shift left
            //        Array.Copy(movingBytes, 0, tempArray, 0, 4);
            //        Array.Copy(tempArray, 1, movingBytes, 0, 3);
            //        movingBytes[3] = x;

            //        return !movingBytes.SequenceEqual(TwoCrLf);
            //    })
            //    .Count() - 1;

            return NaiveSearch(
                new ArraySegment<byte>(bytes, offset, size),
                new byte[]{0x0D, 0x0A, 0x0D, 0x0A}
            );
        }

        private static int NaiveSearch(IReadOnlyList<byte> source, IReadOnlyList<byte> pattern)
        {
            var n = source.Count - 1;
            var m = pattern.Count - 1;

            for (var i = 0; i <= n - m + 1; i++)
            {
                for (var j = 0; j <= m; j++)
                {
                    if (source[i + j] != pattern[j])
                        break;
                    if(j == m)
                        return i;
                }
            }
            return -1;
        }

        private static (HttpVerb, string, string) ParseRequestLine(string s)
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