namespace JournalEntry.UseCases
{
    public class JournalEntry
    {
        public int TransactionId { get; }
        public string Type { get; }

        public JournalEntry(int id, string type)
        {
            TransactionId = id;
            Type = type;
        }
    }
}