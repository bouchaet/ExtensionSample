using Entities;

namespace NonPersistentQueueManager
{
    public class BasicQueueMessageFactory : IQueueMessageFactory
    {
        public IQueueMessage CreateQueueMessage()
        {
            return new BasicQueueMessage();
        }
    }
}
