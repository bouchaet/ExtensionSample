namespace Server.Adapters.Http
{
    internal abstract class Resource
    {
        protected abstract (int status, string content) Get(IHttpRequest request);
        protected abstract (int status, string content) Post(IHttpRequest request);
        protected abstract (int status, string content) Put(IHttpRequest request);
        protected abstract (int status, string content) Delete(IHttpRequest request);

        internal IHttpResponse GetResponse(IHttpRequest request)
        {
            if (request == null)
                return new HttpResponse(HttpStatusCode.BadRequest);

            switch (request.Verb)
            {
                case HttpVerb.Get:
                    return Make(Get(request));
                case HttpVerb.Post:
                    return Make(Post(request));
                case HttpVerb.Put:
                    return Make(Put(request));
                case HttpVerb.Delete:
                    return Make(Delete(request));
                default:
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
