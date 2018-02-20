namespace Http
{
    public class Any : Pattern
    {
        override protected bool Accept(byte item, int pos) => true;
        override public int Length => 1;
    }
}