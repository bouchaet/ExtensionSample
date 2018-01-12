using Entities;
using BfmEventDS = Entities.BfmEvent;

namespace JournalEntry.UseCases
{
    internal class BfmEventCommand : ICommand
    {
        public Port<BfmEventDS> OutPort { get; }

        public BfmEventCommand(Port<BfmEventDS> outputPort)
        {
            OutPort = outputPort;
        }

        public string Name => "event";

        public void Execute()
        {
            OutPort?.Transfer(new BfmEventDS("test", "test"));
        }
    }
}