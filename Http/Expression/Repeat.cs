namespace Http
{
    public class Repeat : Pattern
    {
        private Pattern _rule;
        private int _max;

        public Repeat(Pattern rule, int times)
        {
            _rule = rule;
            _max = times;
            Reset();
        }

        override protected bool Accept(byte item, int pos)
        {
            return _rule.Try(item, pos) && pos < _max;
        }

        override public int Length => _max;

        override public void Reset()
        {
            _rule.Reset();
            base.Reset();
        }
    }
}