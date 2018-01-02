using System;
using System.Collections.Generic;

namespace JournalEntry.UseCases
{
    public interface IArchiver<T>
    {
        void Archive(T obj);
        IEnumerable<T> Select(Func<T, bool> pred = null);
    }
}