using System;

namespace Entities
{
    public interface ILifeManager
    {
        object GetInstance(Type t, object[] constructorArgs);
    }
}