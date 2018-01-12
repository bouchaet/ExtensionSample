using System;

namespace Entities
{
    public interface IListener
    {
        event EventHandler<EventArgs> OnShutdown;
        void Listen();
    }
}