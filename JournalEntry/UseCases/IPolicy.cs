using System.Collections.Generic;

namespace JournalEntry.UseCases
{
    public interface IPolicy
    {
        bool Test(IEnumerable<object> testSubject);
    }
}