using Entities;

namespace Administration.Details
{
    internal class NotFoundCommand : ICommand
    {
        public string Name => "notfound";

        public void Execute()
        {
        }
    }
}