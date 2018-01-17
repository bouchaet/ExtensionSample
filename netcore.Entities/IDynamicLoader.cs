using System.Collections.Generic;

namespace Entities
{
    public interface IDynamicLoader
    {
        IEnumerable<T> GetClass<T>(string filename) where T: class;
    }
}