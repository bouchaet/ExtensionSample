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

        public void AddRoute(string name, Func<string, string> handler) => 
            Routes.Add(new Route(name, new RouteHandler { Handler = handler }));

        public IRouteHandler GetHandler(string route)
        {
            if (Routes.Any(r => r.Name == route))
                return Routes.First(r => r.Name == route).Handler;

            if (Routes.Any(r => "*" == r.Name))
                return Routes.First(r => r.Name == "*").Handler;

            return new RouteHandler { Handler = (str) => "Not Found" };
        }
    }
}