using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Entities;

namespace Server.Adapters.Http
{
    internal class HttpServer
    {
        private readonly int _port;

        private readonly IPAddress _ipAddress;
        private readonly CancellationTokenSource _cts;
        private bool _running;

        private IHttpRouteTable _routetable;

        public IHttpRouteTable RouteTable
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

        public HttpServer(int port)
        {
            _port = port;
            var hostName = Dns.GetHostName();
            var ipHostInfo = Dns.GetHostEntry(hostName);

            _ipAddress = ipHostInfo.AddressList
                .FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);

            if (_ipAddress == null)
                throw new Exception("No IPv4 address for server");

            _cts = new CancellationTokenSource();
            _running = false;
            _routetable = new HttpRouteTable();
            _routetable.Add("/test/:entity/:id", new DebugResource());
        }

        public async Task Start()
        {
            _running = true;
            var listener = new HttpListener(_ipAddress, _port);
            listener.Listen();

            var errors = 0;
            var workerTasks = new List<Task>();
            var listening = false;
            Task<Socket> acceptAsync = null;

            while (!_cts.IsCancellationRequested)
            {
                try
                {
                    if (!listening)
                    {
                        acceptAsync = listener.AcceptAsync();
                        listening = true;
                    }

                    await Task.WhenAny(acceptAsync, Task.Delay(1000));

                    if (acceptAsync.IsCompleted)
                    {
                        listening = false;
                        if (acceptAsync.IsCompletedSuccessfully)
                            workerTasks.Add(ProcessConnection(acceptAsync.Result, _cts.Token));
                    }

                    foreach (var t in workerTasks.Where(
                        t => t.IsCompleted).ToArray())
                    {
                        Debug.Write("Worker removed");
                        workerTasks.Remove(t);
                    }
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

        public void Stop()
        {
            Debug.Write("Http server shutdown requested.");
            _cts.Cancel();
        }

        private Task ProcessConnection(Socket client, CancellationToken token)
        {
            return Task.Run(
                () =>
                {
                    const int arraySize = 2048;
                    var byteArray = new byte[arraySize];
                    var memoryStream = new MemoryStream(arraySize);
                    var netStream = new NetworkStream(client);
                    using (var bufStream = new BufferedStream(netStream))
                    {
                        while (!token.IsCancellationRequested && client.Connected)
                        {
                            while (netStream.DataAvailable)
                            {
                                var bytesRead = netStream.Read(byteArray, 0, byteArray.Length);
                                memoryStream.Write(byteArray, 0, bytesRead);
                                Debug.Write($"Received {bytesRead} bytes.");
                            }

                            if (memoryStream.Position > 0)
                            {
                                var size = (int) memoryStream.Position;
                                var bytes = memoryStream.GetBuffer();
                                memoryStream.Position = 0;

                                var request = HttpMessageParser.Parse(bytes, 0, size);

                                var bytesToSend = MakeResponse(request);
                                bufStream.Write(bytesToSend, 0, bytesToSend.Length);
                                bufStream.Flush();
                            }

                            const int oneSecond = 1000000;
                            client.Poll(oneSecond, SelectMode.SelectRead);
                        }
                    }

                    try
                    {
                        netStream.Close();
                        memoryStream.Close();
                        client.Shutdown(SocketShutdown.Both);
                        client.Close();
                    }
                    catch (SocketException e)
                    {
                        Logger.WriteError($"Exception: {e}");
                    }
                });
        }

        private byte[] MakeResponse(IHttpRequest request)
        {
            var route = _routetable.Find(request.RequestUri);
            request.AddPathParameters(route.PathParameters);

            return route.Resource
                .GetResponse(request)
                .ToBytes();
        }
    }
}