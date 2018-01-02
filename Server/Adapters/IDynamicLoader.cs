using System.Collections.Generic;

namespace Server.Adapters
{
    public interface IDynamicLoader
    {
        IEnumerable<T> GetClass<T>(string filename) where T: class;
    }
}