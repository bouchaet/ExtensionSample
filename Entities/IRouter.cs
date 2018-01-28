using System;

namespace Entities
{
    public interface IRouter
    {
        void AddRoute(string name, Func<string, string> handler);
        IRouteHandler GetHandler(string route);
    }

    public interface IRouteTable<THandler>
    {
        void Add(string relativePath, THandler handler);
        THandler Find(string relativePath);
        void Remove(string relativePath);
    }
}