using System.IO;
using System.Threading.Tasks;

namespace Entities.Http
{
    public abstract class HttpServer
    {
        public abstract IHttpRouteTable RouteTable { set; }
        public bool IsRunning => _running;
        public abstract Task Start();
        public abstract void Stop();

        protected int _port;
        protected bool _running;
        protected HttpServer(int port)
        {
            _port = port;
            _running = false;
        }
    }
}