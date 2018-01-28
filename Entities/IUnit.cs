namespace Entities
{
    public interface IUnit<TIn, TOut>
        where TIn : class
        where TOut : class
    {
        Port<TIn> InPort { get; }
        Port<TOut> OutPort { get; }

        void ConnectFrom(Port<TIn> source);
        void ConnectTo(Port<TOut> target);
    }
}