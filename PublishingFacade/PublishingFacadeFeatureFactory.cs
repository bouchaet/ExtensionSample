using System.Collections.Generic;
using System.Text;
using Entities;

namespace PublishingFacade
{
    public static class PublishingFacadeFeatureFactory
    {
        public static IPublishingFacadeFeature CreateFeature(IContainerBuilder components)
        {
            var queueMgr = components.Get<IQueueManager>();
            var eventQ = queueMgr.CreateQueue("eventQ");
            var cmd = new EventCommand(eventQ);

            return new PublishingFacadeFeature(cmd);
        }
    }

    public interface IPublishingFacadeFeature : IFeature
    {
    }

    internal class PublishingFacadeFeature : IPublishingFacadeFeature
    {
        private readonly ICollection<ICommand> _commands;

        public PublishingFacadeFeature(params ICommand[] commands)
        {
            _commands = commands;
        }

        public string Name => "publishingfacade";
        public IEnumerable<ICommand> Commands => _commands;
        public void Enable()
        {
        }

        public void Disable()
        {
        }
    }

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