namespace Entities
{
    public interface ICommand
    {
        string Name { get; }
        void Execute();
    }
}
