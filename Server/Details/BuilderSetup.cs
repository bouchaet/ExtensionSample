using Entities;

namespace Server.Details
{
    internal class BuilderSetup<T>
        where T : class
    {
        private readonly IContainerBuilder _builder;

        public BuilderSetup(IContainerBuilder builder)
        {
            _builder = builder;
        }

        public void To<TDerived>() where TDerived : T, new()
        {
            _builder.Register<T, TDerived>();
        }

        public void To<TDerived>(params object[] constructorArgs)
            where TDerived : T
        {
            _builder.Register<T, TDerived>(constructorArgs);
        }
    }
}