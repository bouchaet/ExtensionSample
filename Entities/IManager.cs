using System.Collections.Generic;

namespace Entities
{
    public interface IManager<T>
    {
        void Add(T item);
        IEnumerable<T> Items { get; }
        void Clear();
    }
}
