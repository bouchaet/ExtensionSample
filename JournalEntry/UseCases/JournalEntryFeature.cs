using System.Collections.Generic;
using Entities;

namespace JournalEntry.UseCases
{
    public abstract class JournalEntryFeature : IFeature
    {
        public string Name => "journalentry";
        public abstract IEnumerable<ICommand> Commands { get; }
        public abstract void Enable();
        public abstract void Disable();
    }
}