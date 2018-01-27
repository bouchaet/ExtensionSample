using System.Collections.Generic;

namespace Server.Adapters.Http
{
    internal interface IHttpResponse
    {
        HttpStatusCode Status { get; }
        IEnumerable<MessageHeader> Headers { get; }
        byte[] Body { get; }
        byte[] ToBytes();
    }
}
