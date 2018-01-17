using Entities;

namespace NonPersistentQueueManager
{
    public class QueueFactory : IQueueFactory
    {
        private readonly IQueueMessageFactory _factory;

        public QueueFactory(IQueueMessageFactory messageFactory)
        {
            _factory = messageFactory;
        }

        public IQueue CreateQueue(string name)
        {
            return new NonPersistentQueue(name, _factory);
        }
    }
}
