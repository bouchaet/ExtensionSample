namespace Http
{
    public class Concat : Pattern
    {
        private Pattern _left;
        private Pattern _right;

        public Concat(Pattern lhs, Pattern rhs)
        {
            _left = lhs;
            _right = rhs;
            Reset();
        }

        override protected bool Accept(byte item, int pos)
        {
            return _left.Try(item, pos)
                || _right.Try(item, pos - _left.Length);
        }

        override public int Length => _left.Length + _right.Length;

        override public void Reset()
        {
            _left.Reset();
            _right.Reset();
            base.Reset();
        }
    }
}