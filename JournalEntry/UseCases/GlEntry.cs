namespace JournalEntry.UseCases
{
    public class GlEntry
    {
        public GlEntry(int transactionIdentifier, string type)
        {
            TradeIdentifier = transactionIdentifier;
            GlType = type;
        }

        public int TradeIdentifier { get; set; }
        public string GlType { get; set; }

        public override string ToString() =>
            $"{{\"TradeIdentifier\" = \"{TradeIdentifier}\", \"GlType\" = \"{GlType}\"}}";
    }
}