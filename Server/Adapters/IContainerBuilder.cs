namespace Server.Adapters
{
    public interface IContainerBuilder
    {
        T Get<T>() where T : class;

        void Register<T, TDerived>(params object[] args)
            where T : class
            where TDerived : T;
    }
}