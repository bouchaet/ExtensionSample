using System;
using Entities;

namespace Server.Details
{
    internal class SingletonLifeManager : ILifeManager
    {
        private object _singleton;
        private readonly object _syncroot = new object();

        public object GetInstance(Type t, object[] constructorArgs)
        {
            if (_singleton == null)
            {
                lock (_syncroot)
                {
                    if (_singleton == null)
                        _singleton = Activator.CreateInstance(t, constructorArgs);
                }
            }
            return _singleton;
        }
    }
}