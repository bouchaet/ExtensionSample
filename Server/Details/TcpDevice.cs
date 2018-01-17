using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Entities;

namespace Server.Details
{
    internal class TcpDevice : IDevice
    {
        private readonly TcpListener _server;
        private TcpClient _client;
        private int Port { get; }

        public TcpDevice()
            : this(10863)
        {
        }

        public TcpDevice(int port)
        {
            Port = port;
            _server = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
        }

        public void Open()
        {
            _server.Start();
            Logger.WriteInfo($"TcpDevice listening on port {Port}...");
            Logger.WriteInfo("Waiting for connection.");

            _client = _server.AcceptTcpClient(); //todo: blocking call and for a single client...
            Logger.WriteInfo("Connected");
            WriteLine("Welcome. Type 'admin list' to see availables commands.");
        }

        public string ReadLine()
        {
            var bytes = new byte[256];
            var data = string.Empty;
            var stream = _client.GetStream();

            int i;
            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                data += Encoding.ASCII.GetString(bytes, 0, i);

                if (data.EndsWith(Environment.NewLine))
                    break;
            }

            var output = new string(data.Where(c => !char.IsControl(c)).ToArray());
            Logger.WriteInfo($"Received: {output}");
            return output.TrimEnd(Environment.NewLine.ToCharArray());
        }

        public void Write(char[] s, int index, int count)
        {
            if (_client == null)
            {
                Logger.WriteInfo("Client not connected.");
                return;
            }

            var stream = _client.GetStream();
            var bytes = Encoding.ASCII.GetBytes(s);
            stream.Write(bytes, index, bytes.Length);
        }

        public void WriteLine(string s)
        {
            s += Environment.NewLine;
            var charArray = s.ToCharArray();
            Write(charArray, 0, charArray.Length);
        }

        public void Seek(int pos)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            _server.Stop();
            Logger.WriteInfo("TcpDevice stopped.");
        }
    }
}