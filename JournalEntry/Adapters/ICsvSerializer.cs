namespace JournalEntry.Adapters
{
    public interface ICsvSerializer
    {
        string ToCsv<T>(T obj);
    }
}