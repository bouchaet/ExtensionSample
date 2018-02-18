using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace Http
{
    public abstract class Pattern
    {
        abstract public int Length { get; }
        abstract public bool Accept(byte item, int pos);

        public static Pattern Any => new Any();

        public static Pattern Of(char[] array) =>
            array.Select(d => Convert.ToByte(d))
                .Select(b => new One(b) as Pattern)
                .Aggregate((a, n) => a | n);

        public static Pattern operator +(Pattern lhs, Pattern rhs) =>
            new Concat(lhs, rhs);

        public static Pattern operator |(Pattern lhs, Pattern rhs) =>
            new Or(lhs, rhs);

        public static Pattern operator *(Pattern lhs, int times) =>
            new Repeat(lhs, times);

        public static implicit operator Pattern(char @char) =>
            new One(Convert.ToByte(@char));

        public static implicit operator Pattern(char[] sequence) =>
            sequence
                .Select(ch => Convert.ToByte(ch))
                .Select(b => new One(b) as Pattern)
                .Aggregate((a, n) => a + n);


        public static implicit operator Pattern(string sequence) =>
            sequence.ToCharArray();
    }

    public class One : Pattern
    {
        private byte _allowed;
        public One(byte allowed)
        {
            _allowed = allowed;
        }
        override public bool Accept(byte item, int pos)
        {
            return _allowed == item;
        }

        override public int Length => 1;
    }

    public class Repeat : Pattern
    {
        private Pattern _rule;
        private int _max;

        public Repeat(Pattern rule, int times)
        {
            _rule = rule;
            _max = times;
        }

        override public bool Accept(byte item, int pos)
        {
            return _rule.Accept(item, pos) && pos < _max;
        }

        override public int Length => _max;
    }

    public class Any : Pattern
    {
        override public bool Accept(byte item, int pos) => true;
        override public int Length => 1;
    }

    public class Concat : Pattern
    {
        private Pattern _left;
        private Pattern _right;

        public Concat(Pattern lhs, Pattern rhs)
        {
            _left = lhs;
            _right = rhs;
        }

        override public bool Accept(byte item, int pos)
        {
            return pos < _left.Length ? _left.Accept(item, pos)
                : _right.Accept(item, pos - _left.Length);
        }

        override public int Length => _left.Length + _right.Length;
    }

    public class Or : Pattern
    {
        private Pattern _left;
        private Pattern _right;

        public Or(Pattern lhs, Pattern rhs)
        {
            _left = lhs;
            _right = rhs;
        }

        override public int Length => Math.Max(_left.Length, _right.Length);

        override public bool Accept(byte item, int pos)
        {
            return _left.Accept(item, pos) ? true : _right.Accept(item, pos);
        }
    }

    public class ByteExpression
    {
        private Pattern _pattern;

        public ByteExpression(Pattern pattern)
        {
            _pattern = pattern;
        }
        public bool PerfectMatch(byte[] sequence)
        {
            var n = sequence.Length;
            for (var i = 0; i < n; i++)
            {
                if (!_pattern.Accept(sequence[i], i))
                    return false;
            }
            return true;
        }

    }

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
            Pattern.Of(
                new[] { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' }
            );
    }


    public class Lexer : IEnumerator<(string, byte[])?>
    {
        private Stream _stream;
        private (string, byte[])? _current;

        public Lexer(Stream bytes)
        {
            _current = null;
        }

        public (string, byte[])? Current => _current;

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            _stream?.Dispose();
        }

        public bool MoveNext()
        {
            return false;
        }

        public void Reset()
        {
            throw new System.NotImplementedException();
        }
    }
}