using System;
using Entities;
using BfmEventDS = Entities.BfmEvent;

namespace JournalEntry.Details
{
    internal class BfmEventListener : IListener
    {
        public Port<BfmEventDS> OutPort { get; }

        public BfmEventListener(Port<BfmEventDS> outPort)
        {
            OutPort = outPort;
        }

        public event EventHandler<EventArgs> OnShutdown;

        public void Listen()
        {
        }
    }
}