using System;

namespace Entities
{
    public interface IRouter
    {
        void AddRoute(string name, Func<string, string> handler);
        IRouteHandler GetHandler(string route);
    }
}