using Entities;

namespace Administration.Details
{
    internal class CommandPort : Port<ICommand>
    {
        protected override void PreTransfer(ICommand data)
        {
        }

        protected override void PostTransfer(ICommand data)
        {
        }

        protected override void PreReceive(ICommand data)
        {
        }

        protected override void PostReceive(ICommand data)
        {
        }
    }
}
