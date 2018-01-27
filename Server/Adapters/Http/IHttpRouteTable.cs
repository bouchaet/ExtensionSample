using Server.Adapters.Http;

internal interface IHttpRouteTable
{
    void Add(string relativePath, Resource resource);
    void Remove(string relativePath);
    HttpRoute Find(string relativePath);
}
