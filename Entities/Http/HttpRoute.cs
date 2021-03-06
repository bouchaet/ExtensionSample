﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Entities.Http
{
    public class HttpRoute : IRoute
    {
        public string Name => Resource.GetType().Name;

        public Resource Resource { get; private set; }

        public IRouteHandler Handler 
            => Resource.ToRouteHandler();

        private IDictionary<string, string> _pathParameters;

        public static HttpRoute Make(
            Resource resource,
            IDictionary<string, string> pathParameters)
        {
            return new HttpRoute
            {
                Resource = resource,
                _pathParameters = pathParameters ?? new Dictionary<string, string>()
            };
        }

        public IEnumerable<(string key, string value)> PathParameters => _pathParameters.Select(
            kv => (kv.Key, kv.Value));
    }
}
