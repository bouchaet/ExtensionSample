using System.Collections.Generic;

namespace Entities.Http
{
    public interface IHttpRequest
    {
        string Method { get; }
        string Body { get; }
        string RequestUri { get; }
        string HttpVersion { get; }
        IEnumerable<(string, string)> Headers { get; }
        IEnumerable<(string, string)> PathParameters { get; }
        IEnumerable<(string, string)> QueryParameters { get; }

        void AddPathParameters(IEnumerable<(string Key, string Value)> pathParameters);
        void AddQueryParameters(IEnumerable<(string Key, string Value)> pathParameters);
    }
}