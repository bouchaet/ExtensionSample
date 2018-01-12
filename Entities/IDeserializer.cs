namespace Entities
{
    public interface IDeserializer<out TOut>
        where TOut: class
    {
        TOut Deserialize(string s);
    }
}
