namespace JournalEntry.UseCases
{

    public class Mapper : IMapper<JournalEntry, GlEntry>
    {
        public GlEntry Map(JournalEntry obj)
        {
            return new GlEntry(
                obj.TransactionId,
                obj.Type);
        }
    }
}