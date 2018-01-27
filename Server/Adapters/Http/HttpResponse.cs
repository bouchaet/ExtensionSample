using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Adapters.Http
{
    internal class HttpResponse : IHttpResponse
    {
        private readonly ICollection<MessageHeader> _headers;
        private ICollection<byte> _body;

        public HttpStatusCode Status { get; }

        public IEnumerable<MessageHeader> Headers => _headers;

        public byte[] Body => _body.ToArray();

        internal HttpResponse(HttpStatusCode status)
        {
            Status = status;
            _headers = new List<MessageHeader>();
            _body = new List<byte>();
        }

        public void SetContent(string content)
        {
            _body = Encoding.UTF8.GetBytes(content);
            AddHeader("Content-Type", "text/html; charset=UTF-8");
            AddHeader("Content-Length", _body.Count().ToString());
        }

        public void AddHeader(string field, string value)
        {
            _headers.Add(MessageHeader.From(field, value));
        }

        public byte[] ToBytes()
        {
            var startline = $"HTTP/1.1 {Status.Code} {Status.Description}\r\n";
            var headers = Headers
                .Select(h => $"{h.Key}: {h.Value}\r\n")
                .Aggregate((current, next) => current + next);

            const string emptyLine = "\r\n";

            var bytes = Encoding.ASCII
                .GetBytes(startline + headers + emptyLine);

            return bytes.Concat(Body).ToArray();
        }
    }
}
