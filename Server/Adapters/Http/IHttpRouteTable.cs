using Server.Adapters.Http;
using Entities;

internal interface IHttpRouteTable : IRouteTable<HttpRoute>
{
    void Add(string relativePath, Resource resource);
}
