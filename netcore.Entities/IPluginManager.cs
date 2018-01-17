using System.Collections.Generic;

namespace Entities
{
    public interface IPluginManager
    {
        IEnumerable<T> Load<T>() where T : class;
    }
}