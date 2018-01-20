using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Entities;
using Server.Details.Ports;

namespace Server.Adapters
{

    internal class SocketListener : IListener<IDevice>
    {
        private readonly int _port;
        private readonly EventWaitHandle _accepted;
        private readonly EventWaitHandle _stopSignal;
        public event EventHandler<EventArgs> OnShutdown;
        public Port<IDevice> OutPort { get; }
        private readonly object _syncroot = new object();
        private bool _stop;
        public ListenerProtocol ListenerProtocol { get; set; }

        public SocketListener()
            : this(10863)
        {
        }

        public SocketListener(int port)
        {
            _port = port;
            _accepted = new ManualResetEvent(false);
            _stopSignal = new ManualResetEvent(false);
            OutPort = new SimplePort<IDevice>();
            ListenerProtocol = new ChattyListenerProtocol();
        }

        public void Stop()
        {
            _stopSignal.Set();
            lock (_syncroot)
                _stop = true;
        }

        public void Listen()
        {
            Task.Run(() => DoListen());
        }

        private void DoListen()
        {
            //var ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            //var localEndPoint = new IPEndPoint(ipHostInfo.AddressList[0], _port);
            var localEndPoint =
                new IPEndPoint(IPAddress.Parse("127.0.0.1"), _port);

            Logger.WriteInfo($"Local address and port: {localEndPoint}");

            var listener = new Socket(
                localEndPoint.Address.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp
            );

            listener.Bind(localEndPoint);
            listener.Listen(100);

            while (!_stop)
            {
                _accepted.Reset();

                Logger.WriteInfo("Waiting for a connection...");
                listener.BeginAccept(AcceptCallback, listener);
                _accepted.WaitOne();
            }
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            _accepted.Set();
            var listener = (Socket) ar.AsyncState;
            var handler = listener.EndAccept(ar);

            var worker = new Worker
            {
                Socket = handler
            };

            Send(handler, ListenerProtocol.Next(ListenerProtocol.Accept));

            handler.BeginReceive(
                worker.Buffer,
                0,
                worker.BufferSize,
                SocketFlags.None,
                ReadCallback,
                worker
            );
        }

        private void ReadCallback(IAsyncResult ar)
        {
            var worker = (Worker) ar.AsyncState;
            var handler = worker.Socket;

            var read = handler.EndReceive(ar);

            if (read <= 0) return;

            worker.Data.Append(Encoding.ASCII.GetString(worker.Buffer, 0, read));
            var content = worker.Data.ToString();
            if (content.IndexOf(ListenerProtocol.EOF, StringComparison.Ordinal) > -1)
            {
                if (content.StartsWith(ListenerProtocol.Exit))
                {
                    Send(handler, ListenerProtocol.Next(content));
                    worker.Socket.Shutdown(SocketShutdown.Both);
                    worker.Socket.Close();
                    return;
                }

                TransferData(worker);
                var serverResponse = ListenerProtocol.Next(content);
                Send(handler, serverResponse);
                worker.Data.Clear();
            }

            handler.BeginReceive(
                worker.Buffer,
                0,
                worker.BufferSize,
                SocketFlags.None,
                ReadCallback,
                worker
            );
        }

        private void TransferData(Worker worker)
        {
            var content = worker.Data.ToString().TrimEnd(Environment.NewLine.ToCharArray());
            Logger.WriteInfo($"Read {content.Length} bytes from socket. \r\n Data: {content}");

            OutPort.Transfer(new SocketDevice(worker));
        }

        public void Send(Socket handler, string data)
        {
            if (string.IsNullOrEmpty(data)) return;

            var byteData = Encoding.ASCII.GetBytes(data);
            handler.BeginSend(
                byteData,
                0,
                byteData.Length,
                SocketFlags.None,
                SendCallback,
                handler);
        }

        private void SendCallback(IAsyncResult ar)
        {
            var handler = (Socket) ar.AsyncState;
            var bytesSent = handler.EndSend(ar);

            Logger.WriteInfo($"Sent {bytesSent} bytes to client.");
        }
    }

    internal class Worker
    {
        public Socket Socket { get; set; }
        public int BufferSize { get; }
        public byte[] Buffer { get; }
        public StringBuilder Data { get; }

        public Worker()
        {
            BufferSize = 1024;
            Buffer = new byte[BufferSize];
            Data = new StringBuilder();
        }
    }

    internal class SocketDevice : IDevice
    {
        private readonly WeakReference _worker;
        private readonly string _content;

        public SocketDevice(Worker worker)
        {
            _worker = new WeakReference(worker);
            _content = GetContent(worker);
        }

        private string GetContent(Worker worker)
        {
            return worker.Data.ToString().TrimEnd(Environment.NewLine.ToCharArray());
        }

        public void Open()
        {
        }

        public string ReadLine()
        {
            return _content;
        }

        public void Write(char[] s, int index, int count)
        {
            if (!_worker.IsAlive) return;

            var worker = (Worker) _worker.Target;
            var bytes = Encoding.ASCII.GetBytes(s);
            worker.Socket.Send(bytes, SocketFlags.None);
        }

        public void WriteLine(string s)
        {
            if(string.IsNullOrEmpty(s)) return;
            
            s += Environment.NewLine;
            Write(s.ToCharArray(), 0, s.Length);
        }

        public void Seek(int pos)
        {
        }

        public void Close()
        {
        }
    }
}