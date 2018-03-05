using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
            return Algo.NaiveSearch(
                new ArraySegment<byte>(bytes, offset, size),
                new byte[] { 0x0D, 0x0A, 0x0D, 0x0A }
            );
        }

        private static (HttpVerb, string, string) ParseRequestLine(string s)
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

    internal class HttpParser
    {
        //private MemoryStream _mem;

        private IList<Parsable> _tokens;

        public HttpParser()
        {
            _tokens = new List<Parsable>();
        }

        private IEnumerable<Parsable> BuildGrammar()
        {
            byte[] ws = new byte[] { 0b00010100 };
            byte[] crlf = new byte[] { 0b00001101, 0b00001010 };
            byte[] co = Encoding.ASCII.GetBytes(":");

            return new[]
            {
                new Parsable("verb", ws),
                new Parsable("uri", ws),
                new Parsable("version", crlf),
                new Parsable("field", co),
                new Parsable("value", crlf),
                new Parsable("empty", crlf),
                new Parsable("body", new byte[] {0b00000000})
            };
        }


        public void Parse(byte[] bytes, int len)
        {
            int cur = 0;
            int read = 0;

            if(read == 0)
                _tokens[cur].Offset = 0;
            
            for(var i = 0; i < len; i++)
            {
                for(var j = 0; j < _tokens[cur].End.Length; j++)
                {
                    if(i + j > len - 1) break;

                    if(bytes[i+j] != _tokens[cur].End[j])
                        break;

                    if(j == _tokens[cur].End.Length - 1)
                    {
                        _tokens[cur].Size = j + 1;
                        _tokens[++cur].Offset = i + j + 1;
                    }
                }
            }
        }
    }


    internal class TokenExpr
    {
        public byte[] Expression { get; }
        public string Tag { get; }

        public TokenExpr(string tag, byte[] expression)
        {
            Tag = tag;
            Expression = expression;
        }

        public bool Match(byte b)
        {
            return false;
        }
    }

    internal class ByteStream
    {
        private int _available;

        public ByteStream()
        {
            _available = 0;
        }

        public int Available => _available;

        public void Open() {}

        public void Close() {}

        public int Read(byte[] bytes, int offset, int size)
        {
            _available -= size;
            return 0;
        }

        public int Write(byte[] bytes)
        {
            _available = bytes.Length;
            return _available;
        }
    }


    internal class TokenStream
    {
        private HttpLexer _lexer;

        public int Length => 0;

        public TokenStream(HttpLexer lexer)
        {
            _lexer = lexer;
        }

        public int Read((string Tag, byte[] Value)[] tokens, int offset, int size)
        {
            (string Tag, byte[] Value)? token;

            var i = 0;
            for(; i < size && (token = _lexer.MoveNext()) != null; i++)
            {
                tokens[i] = token.Value;
            }

            return i;
        }

        public int Write((string Tag, byte[] Value)[] tokens)
        {
            return 0;
        }
    }

    internal class HttpGrammar
    {
        private TokenStream _tokenStream;

        public HttpGrammar(TokenStream stream)
        {
            _tokenStream = stream;
        }
    }

    internal class HttpLexer
    {
        private MemoryStream _stream;
        private IList<(string, byte[])> _buffer;
        private const string Text = "TEXT";

        public HttpLexer(MemoryStream stream) 
        {
            _stream = stream;
            _buffer = new List<(string, byte[])>();
        }

        public (string Tag, byte[] Value)? MoveNext()
        {
            return NewMethod();
        }

        private (string Tag, byte[] Value)? NewMethod()
        {
            if (!_stream.CanRead) return null;

            var capacity = 1024;
            var bytes = new byte[capacity];

            var currentTag = Text;
            int read = 0;
            for (; (read = _stream.Read(bytes, 0, capacity)) > 0;)
            {
                for (int i = 0; i < read; i++)
                {
                    if (bytes[i] == 0b00010100)
                    {
                        var value = new byte[i];
                        Array.Copy(bytes, value, i + 1);
                        _buffer.Add((currentTag, value));
                        break;
                    }

                    if (bytes[i] == 0b00001101)
                    {

                    }
                }
            }

            return null;
        }
    }

    public static class Tokenizer<T>
    {
        public static IList<T[]> Split(T[] sequence, T[] delimiter) 
        {
            var m = sequence.Length;
            var n = delimiter.Length;
            var result = new List<T[]>();

            var pos = 0;
            for(var i = 0; i < m-n; i++)
            {
                for(var j = 0; j < n; j++)
                {
                    if(!sequence[i+j].Equals(delimiter[j]))
                        break;
                    
                    if(j == n-1)
                    {
                        result.Add(Copy(sequence, pos, i-j-pos));
                        pos = i + n;
                    }
                }
            }
            result.Add(Copy(sequence, pos, m-pos));

            return result;
        }

        private static T[] Copy(T[] source, int pos, int size) =>
            source.Skip(pos).Take(size).ToArray();
    }

    public class HttpStreamParser : IDisposable
    {
        private Stream _stream;
        public HttpStreamParser(Stream stream)
        {
            _stream = stream;
            StartParse();
        }

        private void StartParse()
        {
            var memStream = _stream as MemoryStream;
            var lexer = new HttpLexer(memStream);
            var tokenStream = new TokenStream(lexer);
            var grammar = new HttpGrammar(tokenStream);

        }

        public IHttpRequest GetRequest()
        {
            return null;
        }

        public void Dispose()
        {
            _stream?.Dispose();
        }
    }



    internal class Parsable
    {
        public string Name { get; }
        public byte[] End { get; }
        public ICollection<Parsable> Inners;
        public int Offset { get; set; }
        public int Size { get; set; }

        public Parsable(String name, byte[] end)
            : this(name, end, null)
        { }

        public Parsable(string name, byte[] end, params Parsable[] inners)
        {
            Name = name;
            Inners = inners?.ToList() ?? new List<Parsable>();
            End = end;
        }
    }
}