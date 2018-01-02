using System;
using System.Collections.Generic;

namespace JournalEntry.UseCases
{
    public interface IInfoServicesGateway
    {
        IEnumerable<T> Select<T>(Func<T, bool> pred = null);
    }
}