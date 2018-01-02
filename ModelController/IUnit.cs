using Entities;

namespace ModelBuilder
{
    public interface IUnit<TIn, TOut>
        where TIn : class
        where TOut : class
    {
        Port<TIn> InPort { get; }
        Port<TOut> OutPort { get; }
    }
}