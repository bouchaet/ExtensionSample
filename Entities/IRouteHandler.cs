using System;

namespace Entities
{
    public interface IRouteHandler
    {
        object Map(object input);
    }
}