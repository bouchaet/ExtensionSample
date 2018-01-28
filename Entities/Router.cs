using System;
using System.Collections.Generic;
using System.Linq;

namespace Entities
{
    internal class Router : IRouter
    {
        private ICollection<Route> Routes { get; set; }

        public Router()
        {
            Routes = new List<Route>();
        }

        public void Add(string name, Func<string, string> handler) => 
            Routes.Add(new Route(name, new RouteHandler { Handler = handler }));

        public IRoute Find(string routeName)
        {
            return new Route(routeName, GetHandler(routeName));
        }

        public IRouteHandler GetHandler(string route)
        {
            if (Routes.Any(r => r.Name == route))
                return Routes.First(r => r.Name == route).Handler;

            if (Routes.Any(r => "*" == r.Name))
                return Routes.First(r => r.Name == "*").Handler;

            return new RouteHandler { Handler = (str) => "Not Found" };
        }

        public void Add(string relativePath, IRoute route)
        {
            Add(relativePath, s => route.Handler.Map(s).ToString());
        }

        public void Remove(string relativePath)
        {
            throw new NotImplementedException();
        }
    }
}