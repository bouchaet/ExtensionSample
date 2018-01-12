using System.Collections.Generic;
using System.Linq;
using Entities;

namespace JournalEntry.UseCases
{
    internal class BasicJournalEntryFeature : JournalEntryFeature
    {
        private readonly IListener _listener;
        private readonly IList<ICommand> _commands;

        public BasicJournalEntryFeature(IListener listener, params ICommand[] commands)
        {
            _listener = listener;
            _commands = commands?.ToList() ?? new List<ICommand>();
        }

        public override IEnumerable<ICommand> Commands => _commands;

        public override void Enable()
        {
            _listener.Listen();
        }

        public override void Disable()
        {
        }
    }
}