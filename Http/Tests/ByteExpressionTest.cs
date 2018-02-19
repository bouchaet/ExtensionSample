using System.Text;
using Xunit;

namespace Http
{
    public class ByteExpressionTest
    {
        [Fact]
        internal void ShouldMatchAsciiOneLetterG()
        {
            var expr = new ByteExpression("G");
            Assert.True(expr.PerfectMatch(b("G")));
        }

        [Fact]
        public void ShouldMatchTwoLetterG()
        {
            var expr = new ByteExpression((Pattern)'G' * 2);

            var sequence = b("GG");
            Assert.True(expr.PerfectMatch(sequence));

            sequence = b("aGG");
            Assert.False(expr.PerfectMatch(sequence));
        }

        [Fact]
        public void ShouldMatchSequence()
        {
            var expr = new ByteExpression('G' + (Pattern)'H' * 2);

            var sequence = b("GHH");
            Assert.True(expr.PerfectMatch(sequence));
        }

        [Fact]
        public void ShouldMatchAscii_Get()
        {
            var expr = new ByteExpression("GET");

            Assert.True(expr.PerfectMatch(b("GET")));
        }

        [Fact]
        public void ShouldMatchAscii_GetOrPost()
        {
            var expr = new ByteExpression((Pattern)"GET" | "POST" | "PUT");

            var sequence = b("GET");
            Assert.True(expr.PerfectMatch(sequence), "GET did not match");

            sequence = b("POST");
            Assert.True(expr.PerfectMatch(sequence), "POST did not match");
        }

        [Fact]
        public void ShouldMatchInnerWildCard()
        {
            var expr = new ByteExpression('G' + Any + 'G');

            Assert.True(expr.PerfectMatch(b("G-G")));
            Assert.True(expr.PerfectMatch(b("G G")));
        }

        [Fact]
        public void ShouldMatchHttpVersion()
        {
            var expr = new ByteExpression("HTTP/" + Digit + "." + Digit);

            Assert.True(expr.PerfectMatch(b("HTTP/1.1")), "1.1 failed");
            Assert.True(expr.PerfectMatch(b("HTTP/1.0")), "1.0 failed");
        }

        private byte[] b(string s) => Encoding.ASCII.GetBytes(s);

        private Pattern Any => Pattern.Any;

        private Pattern Digit =>
            Pattern.In(
                new[] { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' }
            );
    }
}