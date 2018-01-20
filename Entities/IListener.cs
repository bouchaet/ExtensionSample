using System;

namespace Entities
{
    public interface IListener
    {
        event EventHandler<EventArgs> OnShutdown;
        void Listen();
        void Stop();
    }

    public interface IListener<T> : IListener where T: class
    {
        Port<T> OutPort { get; }
    }
}