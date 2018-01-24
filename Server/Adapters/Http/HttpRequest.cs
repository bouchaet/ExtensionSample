using System.Collections.Generic;
using System.Net.Sockets;

namespace Server.Adapters.Http
{
    internal class HttpRequest : IHttpRequest
    {
        private Socket _httpClient;

        public HttpVerb Verb { get; set; }

        public string MessageBody { get; set; }

        public string RequestUri { get; set; }

        public string HttpVersion { get; set; }

        public IEnumerable<MessageHeader> Headers => new List<MessageHeader>();

        public HttpRequest(Socket httpClient)
        {
            _httpClient = httpClient;
        }

        public void SetResponse(int code, string body)
        {
        }
    }
}