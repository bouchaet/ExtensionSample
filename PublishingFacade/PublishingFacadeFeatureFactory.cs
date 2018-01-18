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
}