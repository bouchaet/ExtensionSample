using Entities;

namespace Server.Details
{
    public class SimplePort<T> : Port<T> where T : class
    {
        protected override void PostReceive(T data)
        {
        }

        protected override void PostTransfer(T data)
        {
        }

        protected override void PreReceive(T data)
        {
        }

        protected override void PreTransfer(T data)
        {
        }
    }
}