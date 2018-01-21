using System;

namespace Entities
{
    internal class RouteHandler : IRouteHandler
    {
        public Func<string, string> Handler { get; set; }
    }
}