using System;
using System.Collections.Generic;
using System.Linq;
using Entities;

namespace NonPersistentQueueManager
{
    public class VolatileQueueManager : IQueueManager
    {
        private readonly IDictionary<string, IQueue> _queues;

        private readonly IQueueFactory _factory;

        private readonly IDictionary<string, ICollection<Func<IQueueMessage, IActionable>>> _clients
            ;

        public VolatileQueueManager(IQueueFactory factory,
            IDictionary<string, IQueue> queues)
        {
            _factory = factory;
            _queues = queues;
            _clients = new SortedList<string, ICollection<Func<IQueueMessage, IActionable>>>();
        }

        public IQueue CreateQueue(string queuename)
        {
            if (_queues.ContainsKey(queuename))
                return _queues[queuename];

            var newQ = _factory?.CreateQueue(queuename);
            _queues.Add(queuename, newQ);

            if(_clients.ContainsKey(queuename))
                ConnectClient(queuename, _clients[queuename]);

            return newQ;
        }

        public void Subscribe(string queuename, Func<IQueueMessage, IActionable> callback)
        {
            AddClient(queuename, callback);
            ConnectClient(queuename, callback);
        }

        public void Subscribe(string queuename,
            Func<IQueueMessage, (string Error, Action NextStep)> callback)
        {
            Subscribe(queuename, msg =>
            {
                var result = callback?.Invoke(msg);
                return new Actionable
                {
                    Error = result?.Error,
                    NextStep = result?.NextStep
                };
            });
        }

        private void AddClient(string qname, Func<IQueueMessage, IActionable> callback)
        {
            if (!_clients.ContainsKey(qname))
                _clients.Add(qname, new List<Func<IQueueMessage, IActionable>> {callback});
            else
                _clients[qname].Add(callback);
        }

        private void ConnectClient(string queuename, Func<IQueueMessage, IActionable> callback)
        {
            ConnectClient(queuename, new []{callback} );
        }

        private void ConnectClient(string queuename, IEnumerable<Func<IQueueMessage, IActionable>> callbacks)
        {
            if (_queues.ContainsKey(queuename))
                foreach (var cb in callbacks)
                {
                    _queues[queuename].OnReceived += (sender, message) =>
                    {
                        var result = cb?.Invoke(message);
                        if (result?.Error != null && result.Error.Any())
                            Logger.WriteWarning(result.Error);
                        result?.NextStep?.Invoke();
                    };
                    Logger.WriteInfo($"Queue {queuename} connected to callback.");
                }
            else
                Logger.WriteWarning($"Queue {queuename} does not exists yet" +
                                    $" or may never exist.");
        }
    }
}