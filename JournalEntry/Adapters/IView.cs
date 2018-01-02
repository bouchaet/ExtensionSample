namespace JournalEntry.Adapters
{
    public interface IView
    {
        void RenderJson(string obj);
        void RenderCsv(string obj);
    }
}