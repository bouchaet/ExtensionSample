namespace Entities
{
    public interface IPlugin
    {
        void Connect<T>(IManager<T> mgr);
    }
}
