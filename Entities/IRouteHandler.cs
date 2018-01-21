using System;

namespace Entities
{
    public interface IRouteHandler
    {
        Func<string, string> Handler { get; }
    }
}