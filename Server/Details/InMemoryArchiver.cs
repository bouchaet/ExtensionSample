using System;
using System.Collections.Generic;
using System.Linq;
using Entities;
using JournalEntry.UseCases;
using BfmEventDS = Entities.BfmEvent;

namespace Server.Details
{
    internal class InMemoryArchiver : IArchiver<BfmEventDS>
    {
        private readonly ICollection<BfmEventDS> _coll;

        public InMemoryArchiver()
        {
            _coll = new List<BfmEventDS>();
        }

        public void Archive(BfmEventDS obj)
        {
            Logger.WriteInfo($"archiving event {obj}...");
            if (!_coll.Contains(obj))
                _coll.Add(obj);
        }

        public IEnumerable<BfmEventDS> Select(Func<BfmEventDS, bool> pred = null)
        {
            return pred == null ? _coll : _coll.Where(pred);
        }
    }
}