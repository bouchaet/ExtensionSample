namespace Entities
{
    internal class Route
    {
        private string _source;
        private IRouteHandler _handler;

        public Route(string source, IRouteHandler handler)
        {
            _source = source;
            _handler = handler;
        }

        public string Name => _source;
        public IRouteHandler Handler => _handler;
    }

}