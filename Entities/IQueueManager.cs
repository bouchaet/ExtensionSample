using System;

namespace Entities
{
    public interface IActionable
    {
        string Error { get; }
        Action NextStep { get; }
    }

    public sealed class Actionable : IActionable
    {
        public string Error { get; set; }
        public Action NextStep { get; set; }
    }

    public interface IQueueManager
    {
        IQueue CreateQueue(string queuename);

        // todo: the use of handlers creates strong references to the subscribers
        // consider the weak event pattern
        // https://docs.microsoft.com/en-us/dotnet/framework/wpf/advanced/weak-event-patterns
        //
        void Subscribe(string queuename, Func<IQueueMessage, IActionable> callback);

        void Subscribe(
            string queuename,
            Func<IQueueMessage, (string Error, Action NextStep)> callback);

        void Receive(string queuename, byte[] body);
    }

    public interface IQueueMessage
    {
        DateTime Created { get; set; }
        byte[] Body { get; set; }
    }

    public interface IQueue
    {
        event EventHandler<IQueueMessage> OnReceived;
        string Name { get; }
        void Post(byte[] message);
    }

    public interface IQueueFactory
    {
        IQueue CreateQueue(string name);
    }

    public interface IQueueMessageFactory
    {
        IQueueMessage CreateQueueMessage();
    }
}