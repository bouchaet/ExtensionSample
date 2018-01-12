namespace Entities
{
    public interface IContainerBuilder
    {
        /// <summary>
        /// Returns a new instance of T.
        /// </summary>
        /// <typeparam name="T">Type of the class</typeparam>
        /// <returns></returns>
        T Get<T>() where T : class;

        void Register<T, TDerived>(params object[] args)
            where T : class
            where TDerived : T;
    }
}