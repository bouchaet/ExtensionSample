using System;
using System.Linq;
using Entities;

namespace JournalEntry.UseCases
{
    public class Accumulator : IAccumulator
    {
        private readonly IArchiver<BfmEvent> _archiver;
        private readonly IPolicy _policy;
        public event EventHandler OnAccumulationSucceeded;

        public Accumulator(IArchiver<BfmEvent> archiver, IPolicy accumulationPolicy)
        {
            _archiver = archiver;
            _policy = accumulationPolicy;
        }

        public void Accumulate(BfmEvent e)
        {
            _archiver.Archive(e);

            var eventArr = _archiver.Select()?.ToArray();
            if (_policy.Test(eventArr))
                OnAccumulationSucceeded?.Invoke(this, EventArgs.Empty);
        }
    }
}