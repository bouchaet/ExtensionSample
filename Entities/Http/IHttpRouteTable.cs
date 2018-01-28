using Entities;
using Entities.Http;

namespace Entities.Http
{
    public interface IHttpRouteTable : IRouteTable<HttpRoute>
    {
        void Add(string relativePath, Resource resource);
    }
}