namespace Entities
{
    public interface IRouteTable<TRoute>
        where TRoute : IRoute
    {
        void Add(string relativePath, TRoute handler);
        TRoute Find(string relativePath);
        void Remove(string relativePath);
    }
}