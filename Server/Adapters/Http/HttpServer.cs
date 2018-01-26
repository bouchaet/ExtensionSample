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
        private CancellationTokenSource _cts;

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
        }

        public async Task StartAsync()
        {
            var listener = new HttpListener(_ipAddress, _port);
            listener.Listen();

            var errors = 0;
            var workerTasks = new List<Task>();
            var listening = false;
            Task<Socket> acceptAsync = null;

            while (errors < 5 && !_cts.IsCancellationRequested)
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
                            workerTasks.Add(Serve(acceptAsync.Result, _cts));
                    }

                    foreach (var t in workerTasks.Where(t => t.IsCompleted).ToArray())
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
                    //var num = await httpClient.ReceiveAsync(bufferArr, 0);
                    var num = 0;
                    if (!isReceiving)
                    {
                        Debug.Write("Http Listener is waiting for bytes...");
                        receiveTask = httpClient.ReceiveAsync(bufferArr, 0);
                        isReceiving = true;
                    }
                    var tasks = new List<Task>
                    {
                        receiveTask,
                        Task.Delay(5000)
                    };
                    await Task.WhenAny(tasks);
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
                            var request = Parse(membuffer.ToArray());
                            var response = await GetResponse(request);

                            var bytes = Encoding.ASCII.GetBytes(response);
                            var bytesArr = new ArraySegment<byte>(bytes);
                            await httpClient.SendAsync(bytesArr, 0);
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

        private async Task<string> GetResponse(IHttpRequest request)
        {
            await Task.Delay(5);

            var response =
                "<html><div><h2>Welcome to my http server 0.1</h2></div>"+
                "<br>"+
                $"<div>{request}</div></html>";

            return "HTTP/1.1 200 OK\r\n" +
                   "Server: ES-HttpServer v0.1\r\n" +
                   "Content-Type: text/html; charset=UTF-8\r\n" +
                   $"Content-Length: {response.Length}\r\n" +
                   "\r\n" +
                   response;
        }

        private IHttpRequest Parse(byte[] bytes)
        {
            var message = Encoding.ASCII.GetString(bytes);
            var extract = message.Substring(0, Math.Min(message.Length, 500));
            Debug.Write(
                $"Parsing raw request (showing max 500 characters):" +
                $"\r\n{extract}");

            var requestline = new StringReader(message).ReadLine()?.Split(' ');

            if (requestline == null || requestline.Length < 3)
                return new HttpRequest(null)
                {
                    Verb = HttpVerb.Get,
                    RequestUri = "/"
                };

            return new HttpRequest(null)
            {
                Verb = (HttpVerb) Enum.Parse(typeof(HttpVerb), requestline[0].Trim(), true),
                RequestUri = requestline[1],
                HttpVersion = requestline[2]
            };
        }
    }
}