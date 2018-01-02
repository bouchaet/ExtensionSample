using System;
using System.Collections.Generic;

namespace Entities
{
    public abstract class Port<T> where T : class
    {
        private readonly ICollection<Port<T>> _ports;

        public event EventHandler<T> OnDataSent;
        public event EventHandler<T> OnDataReceived;

        protected Port()
        {
            _ports = new List<Port<T>>();
        }

        public void Connect(Port<T> port)
        {
            if (!_ports.Contains(port))
                _ports.Add(port);
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
            PostTransfer(data);
        }

        protected abstract void PreReceive(T data);
        protected abstract void PostReceive(T data);

        public void Receive(T data)
        {
            PreReceive(data);
            OnDataReceived?.Invoke(this, data);
            PostReceive(data);
        }

    }
}
