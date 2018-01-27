using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
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
            private get => _routetable;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(RouteTable));
                if (_running == true)
                    throw new ApplicationException("Http server is running." +
                        " Cannot replace route table at this time.");
                _routetable = value;
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

        private async Task Serve(Socket httpClient, CancellationTokenSource cts)
        {
            const int size = 1024;
            var readbuffer = new byte[size];
            var bufferArr = new ArraySegment<byte>(readbuffer);
            var membuffer = new List<byte>();

            try
            {
                httpClient.SetSocketOption(
                    SocketOptionLevel.Socket,
                    SocketOptionName.KeepAlive,
                    true);

                Task<int> receiveTask = null;
                var isReceiving = false;
                while (httpClient.Connected && !cts.IsCancellationRequested)
                {
                    var num = 0;
                    if (!isReceiving)
                    {
                        Debug.Write("Http Listener is waiting for bytes...");
                        receiveTask = httpClient.ReceiveAsync(bufferArr, 0);
                        isReceiving = true;
                    }

                    await Task.WhenAny(receiveTask, Task.Delay(5000));

                    if (receiveTask.IsCompleted)
                    {
                        num = receiveTask.Result;
                        isReceiving = false;
                    }

                    if (num != 0)
                    {
                        Debug.Write($"Http Listener received {num} bytes from client.");
                        membuffer.AddRange(readbuffer.Take(num));
                        if (num < size)
                        {
                            var request = HttpMessageParser.Parse(membuffer.ToArray(), 0, size);
                            var response = await GetResponse(request);

                            var bytes = Encoding.ASCII.GetBytes(response);
                            var bytesArr = new ArraySegment<byte>(bytes);
                            await httpClient.SendAsync(bytesArr, 0);
                            membuffer.Clear();
                            Debug.Write($"Http Listener sent {bytes.Length} bytes to client.");
                        }
                    }
                    else if (receiveTask.IsCompleted)
                    {
                        Debug.Write(
                            $"Http Listener actively received {num} bytes" +
                            " from client socket. Connection will be closed.");
                        break; // nothing else to share
                    }
                }
                Debug.Write("Closing connection with client.");
                httpClient.Close();
            }
            catch (Exception ex)
            {
                Logger.WriteError($"Connection error while serving client: {ex}");

                if (httpClient.Connected)
                    httpClient.Close();
            }
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
                                var size = (int)memoryStream.Position;
                                var request = HttpMessageParser.Parse(memoryStream.ToArray(), 0, size);
                                memoryStream.Position = 0;

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

        private async Task<string> GetResponse(IHttpRequest request)
        {
            await Task.Delay(5);

            return GetStaticResponse(request);
        }

        private byte[] MakeResponse(IHttpRequest request)
        {
            var resource = _routetable.Find(request.RequestUri);
            var response = resource.GetResponse(request);

            var startline = $"HTTP/1.1 {response.Status.Code} {response.Status.Description}\r\n";
            var headers = response.Headers.Select(h => $"{h.Key}: {h.Value}\r\n");
            var emptyLine = "\r\n";
            var body = response.Body;

            var bytes = Encoding
                .ASCII
                .GetBytes(
                    string.Concat(new {startline, headers, emptyLine})
                );
            return bytes.Concat(body).ToArray();
        }

        private string GetStaticResponse(IHttpRequest request)
        {
            var response =
                "<html><div><h2>Welcome to my http server 0.1</h2></div>" +
                "<br>" +
                $"<div><pre>{request}</pre></div></html>";

            return "HTTP/1.1 200 OK\r\n" +
                   "Server: ES-HttpServer v0.1\r\n" +
                   "Content-Type: text/html; charset=UTF-8\r\n" +
                   $"Content-Length: {response.Length}\r\n" +
                   "\r\n" +
                   response;
        }
    }
}