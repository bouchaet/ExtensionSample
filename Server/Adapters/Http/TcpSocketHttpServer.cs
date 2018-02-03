using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Entities;
using Entities.Http;

namespace Server.Adapters.Http
{
    internal class TcpSocketHttpServer : HttpServer
    {
        private readonly IPAddress _ipAddress;
        private readonly CancellationTokenSource _cts;
        private IHttpRouteTable _routetable;

        public override IHttpRouteTable RouteTable
        {
            set
            {
                if (_running)
                    throw new ApplicationException(
                        "Http server is running." +
                        " Cannot replace route table at this time.");

                _routetable = value ?? throw new ArgumentNullException(nameof(RouteTable));
            }
        }

        private static int GetConfiguredPort =>
            int.TryParse(Environment.GetEnvironmentVariable("es.httpserver.port"), out int port)
                ? port
                : 10867;

        public TcpSocketHttpServer() : this(GetConfiguredPort)
        { }

        public TcpSocketHttpServer(int port) : base(port)
        {
            _ipAddress = GetLocalIp();
            _cts = new CancellationTokenSource();
            _running = false;
            _routetable = new HttpRouteTable();
            _routetable.Add("/test/:entity/:id", new DebugResource());
        }

        private IPAddress GetLocalIp()
        {
            var hostName = Dns.GetHostName();

            return Dns.GetHostEntry(hostName)
                .AddressList
                .FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork)
                ?? throw new Exception("No IPv4 address for server");
        }

        public override async Task Start()
        {
            _running = true;
            var listener = new HttpListener(_ipAddress, Port);
            listener.Listen();

            var errors = 0;
            var workerTasks = new List<Task>();
            var accepting = false;
            Task<Socket> acceptAsync = null;

            while (!_cts.IsCancellationRequested)
            {
                try
                {
                    if (!accepting)
                    {
                        acceptAsync = listener.AcceptAsync();
                        accepting = true;
                    }

                    await Task.WhenAny(acceptAsync, Task.Delay(1000));

                    if (acceptAsync.IsCompleted)
                    {
                        accepting = false;
                        if (acceptAsync.IsCompletedSuccessfully)
                        {
                            var conn = new HttpConnection(acceptAsync.Result,
                                _cts.Token, _routetable);
                            workerTasks.Add(conn.Run());
                        }
                    }

                    workerTasks.RemoveAll(t => t.IsCompleted);
                }
                catch (Exception ex)
                {
                    Logger.WriteError($"Communication error: {ex}");
                    if (++errors <= 5) continue;

                    _cts.Cancel();
                    Logger.WriteError(
                        $"Too many errors ({errors}). Aborting Http server task.");
                }
            }

            Logger.WriteInfo($"Waiting for client termination. {workerTasks.Count} are active.");
            await Task.WhenAll(workerTasks);
            Logger.WriteInfo("Http server is done.");
            _running = false;
        }

        public override void Stop()
        {
            Debug.Write("Http server shutdown requested.");
            _cts.Cancel();
        }
    }
}