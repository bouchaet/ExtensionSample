using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Entities
{
    public abstract class Port<T> where T : class
    {
        private readonly ICollection<Port<T>> _ports;

        public event EventHandler<T> OnDataSent;
        public event EventHandler<T> OnDataReceived;
        public event EventHandler<Port<T>> OnConnected;
        public event Action<T> OnDataSentAsync;
        public event Action<T> OnDataReceivedAsync;

        protected Port()
        {
            _ports = new List<Port<T>>();
        }

        public void Connect(Port<T> port)
        {
            if (!_ports.Contains(port))
                _ports.Add(port);
            
            port.OnConnected(port, this);
        }

        protected abstract void PreTransfer(T data);
        protected abstract void PostTransfer(T data);

        public void Transfer(T data)
        {
            PreTransfer(data);
            foreach (var port in _ports)
            {
                port.Receive(data);
            }
            OnDataSent?.Invoke(this, data);
            DoSentAsync(data);
            PostTransfer(data);
        }

        private void DoSentAsync(T data)
        {
            DoAsync(data, OnDataSentAsync?.GetInvocationList());
        }


        protected abstract void PreReceive(T data);
        protected abstract void PostReceive(T data);

        public void Receive(T data)
        {
            PreReceive(data);
            OnDataReceived?.Invoke(this, data);
            DoDataReceivedAsync(data);
            PostReceive(data);
        }

        private void DoDataReceivedAsync(T data)
        {
            DoAsync(data, OnDataReceivedAsync?.GetInvocationList());
        }

        private static void DoAsync(T data, IEnumerable<Delegate> delegates)
        {
            if (delegates == null) return;

            foreach (var @delegate in delegates)
            {
                var handler = (Action<T>)@delegate;
                Task.Run(() => handler(data));
            }
        }

    }
}
