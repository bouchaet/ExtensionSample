namespace Http
{
    public class ByteExpression
    {
        private Pattern _pattern;

        public ByteExpression(Pattern pattern)
        {
            _pattern = pattern;
        }
        public bool PerfectMatch(byte[] sequence)
        {
            _pattern.Reset();

            var n = sequence.Length;
            for (var i = 0; i < n; i++)
            {
                if (!_pattern.Try(sequence[i], i))
                    return false;
            }
            return true;
        }
    }
}