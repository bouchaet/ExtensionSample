namespace Entities
{
    public interface IModel
    {
        string Name { get; }
        string Compose();
    }
}