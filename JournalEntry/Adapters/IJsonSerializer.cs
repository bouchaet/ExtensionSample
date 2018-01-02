namespace JournalEntry.Adapters
{
    public interface IJsonSerializer
    {
        string ToJson<T>(T obj);
    }
}