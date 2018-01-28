using System;

namespace Entities
{
    public interface IRouter : IRouteTable
    {
        void Add(string name, Func<string, string> handler);
        IRouteHandler GetHandler(string route);
    }

    public interface IRouteTable
    {
        void Add(string routeName, IRoute route);
        IRoute Find(string routeName);
        void Remove(string relativePath);        
    }
}