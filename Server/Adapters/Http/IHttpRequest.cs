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

        void SetResponse(int code, string body);
    }
}