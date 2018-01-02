using System;
using System.Collections.Generic;
using Server.Adapters;

namespace Server.Details
{
    public class ContainerBuilder : IContainerBuilder
    {
        private readonly IDictionary<Type, Type> _map;
        private readonly IDictionary<Type, object[]> _args;

        public ContainerBuilder()
        {
            _map = new Dictionary<Type, Type>();
            _args = new Dictionary<Type, object[]>();
        }

        public T Get<T>() where T : class
        {
            if (_map.ContainsKey(typeof(T)))
                return (T) Activator.CreateInstance(_map[typeof(T)], _args[typeof(T)]);

            throw new Exception("Type is unknown");
        }

        public void Register<TAbstract, TConcrete>(params object[] constructorArgs)
            where TAbstract : class
            where TConcrete : TAbstract
        {
            if (_map.ContainsKey(typeof(TAbstract)))
                throw new ApplicationException("Type already registered.");

            if (typeof(TConcrete).IsAbstract || typeof(TConcrete).IsInterface)
                throw new ApplicationException("Concrete type cannot be abstract");

            _map.Add(typeof(TAbstract), typeof(TConcrete));
            _args.Add(typeof(TAbstract), constructorArgs);
        }

        public BuilderSetup<T> Set<T>() where T : class
        {
            if (typeof(T).IsInterface || typeof(T).IsAbstract)
                return new BuilderSetup<T>(this);

            throw new ApplicationException("Type must be abstract");
        }
    }
}