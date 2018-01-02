using System.Collections.Generic;
using System.Linq;
using Entities;
using JournalEntry.UseCases;

namespace JournalEntry.Adapters
{
    public class Controller
    {
        public Port<BfmEvent> EventPort { get; }
        public Port<IEnumerable<GlEntry>> GlEntryPort { get; }

        public Controller(Port<BfmEvent> eventPort,
            Port<IEnumerable<GlEntry>> glEntryPort,
            IAccumulator accumulator,
            IMapper<UseCases.JournalEntry, GlEntry> mapper,
            IInfoServicesGateway gateway)
        {
            EventPort = eventPort;
            GlEntryPort = glEntryPort;

            EventPort.OnDataReceived += (sender, e) => accumulator.Accumulate(e);
            accumulator.OnAccumulationSucceeded += (sender, args) =>
            {
                var glEntries = gateway
                    .Select<UseCases.JournalEntry>()
                    .Select(mapper.Map)
                    .ToArray();

                if (glEntries.Any())
                    GlEntryPort.Transfer(glEntries);
            };
        }
    }
}