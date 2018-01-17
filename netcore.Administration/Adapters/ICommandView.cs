using System.Collections.Generic;

namespace Administration.Adapters
{
    internal interface ICommandView
    {
        void ShowAll(IEnumerable<string> cmds);
    }
}