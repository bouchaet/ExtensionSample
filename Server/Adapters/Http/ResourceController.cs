using System;
using Entities;
using Entities.Http;

namespace Server.Adapters.Http
{
    internal class ResourceController
    {
        private Resource _inner;
        public ResourceController(Resource inner)
        {
            _inner = inner;
        }

        internal IHttpResponse GetResponse(IHttpRequest request)
        {
            if (request == null)
                return new HttpResponse(HttpStatusCode.BadRequest);

            try
            {
                return Make(_inner.Map(request));
            }
            catch(ApplicationException ex)
            {
                Debug.Write($"Exception: {ex}");
                return new HttpResponse(HttpStatusCode.BadRequest);
            }
        }

        private IHttpResponse Make((int status, string content) result)
        {
            var response = new HttpResponse(HttpStatusCode.From(result.status));
            response.SetContent(result.content);

            return response;
        }
    }
}