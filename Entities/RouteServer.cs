using System;
using Entities;

namespace Entities
{
    public abstract class RouteServer
    {
        private IRouter _router;
        private IListener<IDevice> _listener;

        protected RouteServer(IRouter router,
            IListener<IDevice> listener,
            ListenerProtocol protocol)
        {
            _router = router ?? new Router();
            _listener = listener;
            _listener.ListenerProtocol = protocol;
            _listener.OutPort.Connect(new DeviceResponder(_router));
        }

        protected RouteServer(IListener<IDevice> listener,
            ListenerProtocol protocol)
            : this(new Router(), listener, protocol)
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
            _router.AddRoute(routename, routeHandler);
        }
    }
}