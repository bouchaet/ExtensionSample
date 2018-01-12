using System.Collections.Generic;
using Entities;

namespace Administration
{
    internal class StringListPort : Port<IList<string>>
    {
        protected override void PreTransfer(IList<string> data)
        {
        }

        protected override void PostTransfer(IList<string> data)
        {
        }

        protected override void PreReceive(IList<string> data)
        {
        }

        protected override void PostReceive(IList<string> data)
        {
        }
    }
}