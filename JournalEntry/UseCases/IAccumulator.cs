using System;
using Entities;

namespace JournalEntry.UseCases
{
    public interface IAccumulator
    {
        event EventHandler OnAccumulationSucceeded;
        void Accumulate(BfmEvent e);
    }
}