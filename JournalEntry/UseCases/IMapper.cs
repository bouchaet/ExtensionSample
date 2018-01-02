namespace JournalEntry.UseCases
{
    public interface IMapper<in TIn, out TOut>
        where TIn : class
        where TOut : class
    {
        TOut Map(TIn obj);
    }
}