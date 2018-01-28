using System;
using System.Collections.Generic;
using System.Linq;
using Entities.Http;

namespace Server.Adapters.Http
{
    internal class HttpRequest : IHttpRequest
    {
        private ICollection<MessageHeader> _headers;
        private readonly Dictionary<string, string> _pathParameters;
        private readonly Dictionary<string, string> _queryParameters;


        public HttpVerb Verb { get; set; }

        public string Method { get => Enum.GetName(typeof(HttpVerb), Verb).ToUpper(); }

        public string Body { get; set; }

        public string RequestUri { get; set; }

        public string HttpVersion { get; set; }

        public void AddPathParameters(IEnumerable<(string Key, string Value)> pathParameters)
        {
            AddParametersTo(_pathParameters, pathParameters);
        }

        public void AddQueryParameters(IEnumerable<(string Key, string Value)> queryParameters)
        {
            AddParametersTo(_queryParameters, queryParameters);
        }

        private static void AddParametersTo(
            IDictionary<string, string> target,
            IEnumerable<(string Key, string Value)> source)
        {
            foreach (var param in source)
            {
                target.Add(param.Key, param.Value);
            }
        }

        public IEnumerable<MessageHeader> MessageHeaders
        {
            get => _headers;
            set => _headers = new List<MessageHeader>(value);
        }

        public IEnumerable<(string, string)> Headers => _headers.Select(
            x => (x.Key, x.Value));

        public IEnumerable<(string, string)> PathParameters => _pathParameters.Select(
            kv => (kv.Key, kv.Value));
        public IEnumerable<(string, string)> QueryParameters => _queryParameters.Select(
            kv => (kv.Key, kv.Value));

        public HttpRequest()
        {
            _pathParameters = new Dictionary<string, string>();
            _queryParameters = new Dictionary<string, string>();
        }

        public void SetResponse(int code, string body)
        {
        }

        public override string ToString()
        {
            return $"{Verb.ToString().ToUpper()} {RequestUri} {HttpVersion}" +
                   "\r\n" +
                   _headers
                       .Select(h => $"{h.Key}:{h.Value}\r\n")
                       .Aggregate((current, next) => current + next) +
                   Body.Substring(0, Math.Min(Body.Length, 500));
        }
    }
}