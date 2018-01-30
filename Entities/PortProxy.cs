namespace Entities
{
    internal class PortProxy<T> : Port<T> where T : class
    {
        private PortProxy()
        { }

        public static Port<T> LazyWrap(Port<T> port)
        {
            var proxy = new PortProxy<T>();
            proxy.OnConnected += (s, e) => ((Port<T>)s).Connect(e); 
            return proxy;
        }

        protected override void PostReceive(T data)
        { }

        protected override void PostTransfer(T data)
        { }

        protected override void PreReceive(T data)
        { }

        protected override void PreTransfer(T data)
        { }
    }
}