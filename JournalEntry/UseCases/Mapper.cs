namespace JournalEntry.UseCases
{

    public class Mapper : IMapper<JournalEntry, PartnerGlEntry>
    {
        public PartnerGlEntry Map(JournalEntry obj)
        {
            return new PartnerGlEntry(
                obj.TransactionId,
                obj.Type);
        }
    }
}