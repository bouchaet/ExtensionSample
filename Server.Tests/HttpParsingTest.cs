using System;
using System.IO;
using System.Text;
using Server.Adapters.Http;
using Xunit;

namespace Server.Tests
{
    public class HttpParsingTest
    {
        [Fact]
        public void ShouldParseSimpleGet()
        {
            var getMessage = Encoding.ASCII.GetBytes(
                "GET / HTTP/1.1\r\n" +
                "Content-Type:text/html; charset=utf-8\r\n" +
                "\r\n"
            );

            var msgStream = new MemoryStream(getMessage);

            var parser = new HttpStreamParser(msgStream);
            
            Assert.True(parser != null);
        }

        [Fact]
        public void ShouldTokenizeOnWhitespace()
        {
            var msg = Encoding.ASCII.GetBytes(
                "GET /test/123 HTTP/1.1"
            );

            var tokens = Tokenizer<byte>.Split(msg, new byte[]{0b00100000});

            Assert.Equal(3, tokens.Count);
            Assert.Equal("GET", Encoding.ASCII.GetString(tokens[0]));
            Assert.Equal("/test/123", Encoding.ASCII.GetString(tokens[1]));
            Assert.Equal("HTTP/1.1", Encoding.ASCII.GetString(tokens[2]));
        }
    }
}
