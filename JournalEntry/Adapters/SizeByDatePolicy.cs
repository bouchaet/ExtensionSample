using System.Collections.Generic;
using System.Linq;
using JournalEntry.UseCases;

namespace JournalEntry.Adapters
{
    public class SizeByDatePolicy : IPolicy
    {
        private readonly int _size;

        public SizeByDatePolicy(int size)
        {
            _size = size;
        }

        public bool Test(IEnumerable<object> enumerable)
        {
            return enumerable.Count() >= _size;
        }
    }
}