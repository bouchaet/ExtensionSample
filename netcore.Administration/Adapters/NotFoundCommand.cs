using Entities;

namespace Administration.Adapters
{
    internal class NotFoundCommand : ICommand
    {
        public string Name => "notfound";

        public void Execute()
        {
        }
    }
}