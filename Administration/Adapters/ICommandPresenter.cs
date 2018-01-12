using System.Collections.Generic;

namespace Administration.Adapters
{
    internal interface ICommandPresenter
    {
        void ShowCommands(IEnumerable<string> commands);
        void AttachView(ICommandView view);
    }
}