using System;
using System.Collections.Generic;
using System.Linq;
using JournalEntry.UseCases;
using JournalEntryDS = JournalEntry.UseCases.JournalEntry;

namespace Server.Details
{
    public class InMemoryInfoSvcGateway : IInfoServicesGateway
    {
        private readonly IDictionary<Type, ICollection<object>> _store;

        public InMemoryInfoSvcGateway()
        {
            _store = new Dictionary<Type, ICollection<object>>
            {
                {
                    typeof(JournalEntryDS),
                    new[]
                    {
                        new JournalEntryDS(1, "type1")
                    }
                }
            };
        }

        public IEnumerable<T> Select<T>(Func<T, bool> pred = null)
        {
            return _store.ContainsKey(typeof(T))
                ? _store[typeof(T)].Cast<T>()
                : Enumerable.Empty<T>();
        }
    }
}