using Entities;

namespace Administration.UseCases
{
    internal class HistoryCommand : ICommand
    {
        public string Name => "history";

        public void Execute()
        {
        }
    }
}