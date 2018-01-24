using System;
using Entities;

namespace Http
{
    public class HttpServer : RouteServer
    {
        public HttpServer(IListener<IDevice> listener)
        : base(listener, new HttpProtocol())
        {
            listener.Configure("port", 10867);

        }
    }
}
