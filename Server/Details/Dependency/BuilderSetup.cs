using Entities;

namespace Server.Details.Dependency
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
            _builder.Register<T, TDerived>(new PerCallLifeManager());
        }

        public void To<TDerived>(params object[] constructorArgs)
            where TDerived : T
        {
            _builder.Register<T, TDerived>(new PerCallLifeManager(), constructorArgs);
        }

        public void To<TDerived>(ILifeManager mgr, params object[] constructorArgs)
            where TDerived : T
        {
            _builder.Register<T, TDerived>(mgr, constructorArgs);
        }
    }
}