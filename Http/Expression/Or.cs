using System;

namespace Http
{
    public class Or : Pattern
    {
        private Pattern _left;
        private Pattern _right;

        public Or(Pattern lhs, Pattern rhs)
        {
            _left = lhs;
            _right = rhs;
            Reset();
        }

        override public int Length => Math.Max(_left.Length, _right.Length);

        override protected bool Accept(byte item, int pos)
        {
            return _left.Try(item, pos) || _right.Try(item, pos);
        }

        override public void Reset()
        {
            _left.Reset();
            _right.Reset();
            base.Reset();
        }
    }
}