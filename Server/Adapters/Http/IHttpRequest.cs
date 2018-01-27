using System.Collections.Generic;

namespace Server.Adapters.Http
{
    internal interface IHttpRequest
    {
        HttpVerb Verb { get; }
        string MessageBody { get; }
        string RequestUri { get; }
        string HttpVersion { get; }
        IEnumerable<MessageHeader> Headers { get; }
        IEnumerable<(string, string)> PathParameters { get; }
        IEnumerable<(string, string)> QueryParameters { get; }

        void AddPathParameters(IEnumerable<(string Key, string Value)> pathParameters);
        void AddQueryParameters(IEnumerable<(string Key, string Value)> pathParameters);

        string Body { get; set; }
    }
}