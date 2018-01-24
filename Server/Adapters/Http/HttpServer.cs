using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Server.Adapters.Http
{
    internal class HttpServer
    {
        private readonly int _port;

        private readonly IPAddress _ipAddress;

        public HttpServer(int port)
        {
            _port = port;
            var hostName = Dns.GetHostName();
            var ipHostInfo = Dns.GetHostEntry(hostName);

            _ipAddress = ipHostInfo.AddressList
                .FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);

            if (_ipAddress == null)
                throw new Exception("No IPv4 address for server");
        }

        public async Task StartAsync()
        {
            var listener = new HttpListener(_ipAddress, _port);
            listener.Listen();

            while (true)
            {
                try
                {
                    Debug.Write("Http server is waiting for connection...");
                    var httpClient = await listener.AcceptAsync();
                    Debug.Write("Http server: client accepted.");
                    var task = Serve(httpClient);
                }
                catch (Exception ex)
                {
                    Logger.WriteError($"Communication error: {ex}");
                }
            }
        }

        private async Task Serve(Socket httpClient)
        {
            const int size = 512;
            var readbuffer = new byte[size];
            var bufferArr = new ArraySegment<byte>(readbuffer);
            var membuffer = new List<byte>();

            try
            {
                httpClient.SetSocketOption(
                    SocketOptionLevel.Socket,
                    SocketOptionName.KeepAlive,
                    true);

                while (httpClient.Connected)
                {
                    Debug.Write($"Http Listener is waiting for bytes...");
                    var num = await httpClient.ReceiveAsync(bufferArr, 0);
                    Debug.Write($"Http Listener received {num} bytes from client.");
                    if (num != 0)
                    {
                        membuffer.AddRange(readbuffer.Take(num));
                        if (num < size)
                        {
                            var request = Parse(membuffer.ToArray());
                            //var response = await GetResponse(request);
                            var response = GetResponse(request);
                            var bytes = Encoding.ASCII.GetBytes(response);
                            var bytesArr = new ArraySegment<byte>(bytes);
                            await httpClient.SendAsync(bytesArr, 0);
                            Debug.Write($"Http Listener sent {bytes.Length} bytes to client.");
                        }
                    }
                    else
                    {
                        break; // nothing else to share
                    }
                }
                httpClient.Close();
            }
            catch (Exception ex)
            {
                Logger.WriteError($"Connection error while serving client: {ex}");

                if (httpClient.Connected)
                    httpClient.Close();
            }
        }

        //private async Task<string> GetResponse(IHttpRequest request)
        private string GetResponse(IHttpRequest request)
        {
            var response = "<html>Weicome to my http server 0.1</html>";

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

            return new HttpRequest(null)
            {
                Verb = HttpVerb.Get,
                RequestUri = "/"
            };
        }
    }
}