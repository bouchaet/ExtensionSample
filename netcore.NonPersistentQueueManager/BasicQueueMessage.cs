using System;
using Entities;

namespace NonPersistentQueueManager
{
    internal class BasicQueueMessage : IQueueMessage
    {
        public DateTime Created { get; set; }
        public byte[] Body { get; set; }
    }
}