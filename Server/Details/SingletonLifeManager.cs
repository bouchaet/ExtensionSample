using System;
using Entities;

namespace Server.Details
{
    internal class SingletonLifeManager : ILifeManager
    {
        private static object _singleton;
        private static readonly object Syncroot = new object();

        public object GetInstance(Type t, object[] constructorArgs)
        {
            if (_singleton == null)
            {
                lock (Syncroot)
                {
                    if (_singleton == null)
                        _singleton = Activator.CreateInstance(t, constructorArgs);
                }
            }
            return _singleton;
        }
    }
}