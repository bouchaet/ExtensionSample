using System;
using Entities;

namespace Entities
{
    public abstract class RouteServer
    {
        private IRouter _router;
        private IListener<IDevice> _listener;

        protected RouteServer(IRouter router,
            IListener<IDevice> listener)
        {
            _router = router ?? new Router();
            _listener = listener;
            _listener.OutPort.Connect(new DeviceResponder(_router));
        }

        protected RouteServer(IListener<IDevice> listener)
            : this(new Router(), listener)
        { }

        public void Start()
        {
            _listener.Listen();
        }

        public void Stop()
        {
            _listener.Stop();
        }

        public void Add(string routename, Func<string, string> routeHandler)
        {
            _router.Add(routename, routeHandler);
        }
    }
}