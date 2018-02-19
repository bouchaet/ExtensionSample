namespace Http
{
    public class One : Pattern
    {
        private byte _allowed;
        public One(byte allowed)
        {
            _allowed = allowed;
            Reset();
        }
        override protected bool Accept(byte item, int pos)
        {
            return _allowed == item;
        }

        override public int Length => 1;
    }
}