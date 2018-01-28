namespace Entities
{
    public interface IRoute
    {
        string Name { get; }
        IRouteHandler Handler {get;}
    }

}