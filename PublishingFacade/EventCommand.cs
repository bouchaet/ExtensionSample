using System.Text;
using Entities;

namespace PublishingFacade
{
    internal class EventCommand : ICommand
    {
        private readonly IQueue _eventQ;

        public EventCommand(IQueue eventQ)
        {
            _eventQ = eventQ;
        }

        public string Name => "send-event";
        public void Execute()
        {
            var msg = Encoding.UTF8.GetBytes("test#test");
            _eventQ.Post(msg);
        }
    }
}