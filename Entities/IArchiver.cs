using System;
using System.Collections.Generic;

namespace Entities
{
    public interface IArchiver<T>
    {
        void Archive(T obj);
        IEnumerable<T> Select(Func<T, bool> pred = null);
    }
}