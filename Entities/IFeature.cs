using System.Collections.Generic;

namespace Entities
{
    public interface IFeature
    {
        string Name { get; }
        IEnumerable<ICommand> Commands { get; }
        void Enable();
        void Disable();
    }
}
