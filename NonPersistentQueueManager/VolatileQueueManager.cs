using System;
using System.Collections.Generic;
using Entities;

namespace NonPersistentQueueManager
{

    internal class BasicQueueMessage : IQueueMessage
    {
        public DateTime Created { get; set; }
        public byte[] Body { get; set; }
    }

    internal class QueueFactory : IQueueFactory
    {
        public IQueue CreateQueue(string name)
        {
            return new NonPersistentQueue(name, new BasicQueueMessageFactory());
        }
    }

    internal class VolatileQueueManager : IQueueManager
    {
        private readonly IDictionary<string, IQueue> _queues;

        private readonly IQueueFactory _factory;

        public VolatileQueueManager(IQueueFactory factory,
            IDictionary<string, IQueue> queues)
        {
            _factory = factory;
            _queues = queues;
        }

        public IQueue CreateQueue(string queuename)
        {
            if (_queues.ContainsKey(queuename))
                return _queues[queuename];

            var newQ = _factory?.CreateQueue(queuename);
            _queues.Add(queuename, newQ);
            return newQ;
        }

        public void Subscribe(string queuename, Func<IQueueMessage, IActionable> callback)
        {
            if (_queues.ContainsKey(queuename))
                _queues[queuename].OnReceived += (sender, message) =>
                {
                    var action = callback?.Invoke(message);
                    if (action?.Error != null)
                        Logger.WriteInfo(action.Error);
                    action?.NextStep?.Invoke();
                };
        }
    }

    internal class NonPersistentQueue : IQueue
    {
        private readonly IQueueMessageFactory _factory;
        public event EventHandler<IQueueMessage> OnReceived;
        public string Name { get; }

        public NonPersistentQueue(string queueName, IQueueMessageFactory factory)
        {
            Name = queueName;
            _factory = factory;
        }

        public void Post(byte[] message)
        {
            var msg = _factory.CreateQueueMessage();
            msg.Body = message;
            msg.Created = DateTime.Now;

            OnReceived?.Invoke(this, msg);
        }
    }

    internal class BasicQueueMessageFactory : IQueueMessageFactory
    {
        public IQueueMessage CreateQueueMessage()
        {
            return new BasicQueueMessage();
        }
    }
}
