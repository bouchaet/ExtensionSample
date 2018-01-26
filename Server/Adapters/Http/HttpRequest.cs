using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace Server.Adapters.Http
{
    internal class HttpRequest : IHttpRequest
    {
        private Socket _httpClient;
        private ICollection<MessageHeader> _headers;

        public HttpVerb Verb { get; set; }

        public string MessageBody { get; set; }

        public string RequestUri { get; set; }

        public string HttpVersion { get; set; }

        public string Body { get; set; }

        public IEnumerable<MessageHeader> Headers
        {
            get => _headers;
            set => _headers = new List<MessageHeader>(value);
        }

        public HttpRequest(Socket httpClient)
        {
            _httpClient = httpClient;
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