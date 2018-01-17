using System;
using System.Collections.Generic;
using Entities;

namespace Server.Details.Dependency
{
    internal class ContainerBuilder : IContainerBuilder
    {
        private readonly IDictionary<Type, Type> _map;
        private readonly IDictionary<Type, object[]> _args;
        private readonly IDictionary<Type, ILifeManager> _lm;

        public ContainerBuilder()
        {
            _map = new Dictionary<Type, Type>();
            _args = new Dictionary<Type, object[]>();
            _lm = new Dictionary<Type, ILifeManager>();
        }

        public T Get<T>() where T : class
        {
            var abstractType = typeof(T);
            if (_map.ContainsKey(abstractType))
                return (T)_lm[abstractType].GetInstance(_map[abstractType], _args[abstractType]);

            throw new Exception($"Type {abstractType} is unknown");
        }

        public void Register<TAbstract, TConcrete>(ILifeManager mgr, params object[] constructorArgs)
            where TAbstract : class
            where TConcrete : TAbstract
        {
            if (_map.ContainsKey(typeof(TAbstract)))
                throw new ApplicationException("Type already registered.");

            if (typeof(TConcrete).IsAbstract || typeof(TConcrete).IsInterface)
                throw new ApplicationException("Concrete type cannot be abstract");

            _map.Add(typeof(TAbstract), typeof(TConcrete));
            _args.Add(typeof(TAbstract), constructorArgs);
            _lm.Add(typeof(TAbstract), mgr);
        }

        public BuilderSetup<T> Set<T>() where T : class
        {
            if (typeof(T).IsInterface || typeof(T).IsAbstract)
                return new BuilderSetup<T>(this);

            throw new ApplicationException("Type must be abstract");
        }
    }
}