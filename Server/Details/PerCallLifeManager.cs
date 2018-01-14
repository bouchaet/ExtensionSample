using System;
using Entities;

namespace Server.Details
{
    internal class PerCallLifeManager : ILifeManager
    {
        public object GetInstance(Type t, object[] constructorArgs)
        {
            return Activator.CreateInstance(t, constructorArgs);
        }
    }
}