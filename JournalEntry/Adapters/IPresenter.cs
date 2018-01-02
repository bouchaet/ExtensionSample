using System.Collections.Generic;

namespace JournalEntry.Adapters
{
    public interface IPresenter<T>
    {
        void AttachView(IView view);
        void ShowAllElements(IEnumerable<T> enumerable);
    }
}