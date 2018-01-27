using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Adapters.Http
{
    internal class HttpRouteTable : IHttpRouteTable
    {
        private readonly IDictionary<string, HttpRoute> _routeTable;
        private Resource NullResouce { get; }

        public HttpRouteTable()
        {
            _routeTable = new Dictionary<string, HttpRoute>();
            NullResouce = new NullResource();
        }

        public void Add(string relativePath, Resource resource)
        {
            if (!relativePath.StartsWith("/"))
                throw new ArgumentException(
                    nameof(relativePath),
                    $"Must be a relative path that starts with '/'");

            var (pathParameters, normalizedPath) = NormalizePath(relativePath);
            var route = HttpRoute.Make(resource, pathParameters.ToDictionary(p => p), null);

            if (_routeTable.ContainsKey(normalizedPath))
                _routeTable[normalizedPath] = route;

            _routeTable.Add(normalizedPath, route);
        }

        public HttpRoute Find(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
                return HttpRoute.Make(NullResouce, null, null);

            return (from r in _routeTable
                       let m = Match(r.Key, relativePath)
                       where m.IsEqual
                       let pathKeyValues = r.Value.PathParameters.Select(
                               (kv, i) => new {Value = kv, Index = i})
                           .ToDictionary(it => it.Value.key, it => m.PathParameterValues[it.Index])
                       select HttpRoute.Make(r.Value.Resource, pathKeyValues, null))
                   .FirstOrDefault() ??
                   HttpRoute.Make(NullResouce, null, null);
        }

        private static (bool IsEqual, string[] PathParameterValues) Match(string lhs, string rhs)
        {
            var False = (false, new string[] { });

            var lhsParts = lhs.Split("/", StringSplitOptions.RemoveEmptyEntries);
            var rhsParts = rhs.Split("?")[0].Split("/", StringSplitOptions.RemoveEmptyEntries);

            if (lhsParts.Length != rhsParts.Length)
                return False;

            var pathValues = new List<string>();
            for (var i = 0; i < lhsParts.Length; i++)
            {
                if (lhsParts[i] == "*")
                {
                    pathValues.Add(rhsParts[i]);
                    continue;
                }

                if (string.Compare(lhsParts[i], rhsParts[i], StringComparison.Ordinal) != 0)
                    return False;
            }

            return (true, pathValues.ToArray());
        }

        private static ICollection<string> EmptyParams => new string[] { };

        private static (ICollection<string> pathParameters, string path)
            NormalizePath(string relativePath)
        {
            var trimmedPath = relativePath.Trim();

            if (trimmedPath == "*")
                return (EmptyParams, trimmedPath);

            var parts = trimmedPath
                .Replace('\\', '/')
                .Split("/", StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0)
                return (EmptyParams, trimmedPath);

            if (!parts.Any(p => p.StartsWith(":")))
                return (EmptyParams, trimmedPath);

            var pathParams = new List<string>();
            var sb = new StringBuilder();
            foreach (var p in parts)
            {
                if (p.StartsWith(":"))
                {
                    pathParams.Add(p.Substring(1));
                    sb.Append("/*");
                    continue;
                }

                sb.Append($"/{p}");
            }

            return (pathParams, sb.ToString());
        }

        public void Remove(string relativePath)
        {
            throw new NotImplementedException();
        }
    }
}