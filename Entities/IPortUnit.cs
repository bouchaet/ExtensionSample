namespace Entities
{
    public abstract class PortUnit<TIn, TOut>
        where TIn : class
        where TOut : class
    {
        private Port<TIn> _input;
        private Port<TOut> _output;

        protected PortUnit(Port<TIn> input, Port<TOut> output)
        {
            _input = input;
            _output = output;
        }

        public void ConnectTo(Port<TOut> target)
            => _output?.Connect(target);

        public Port<TIn> InputProxy
            => PortProxy<TIn>.LazyWrap(_input);
    }
}