using Entities.Http;

namespace Server.Adapters.Http
{
    internal class NullResource : Resource
    {
        protected override (int status, string content) Delete(IHttpRequest request)
            => MakeNotFound;

        protected override (int status, string content) Get(IHttpRequest request)
            => MakeNotFound;

        protected override (int status, string content) Post(IHttpRequest request)
            => MakeNotFound;

        protected override (int status, string content) Put(IHttpRequest request)
            => MakeNotFound;

        private static (int, string) MakeNotFound => (404, "Sorry, nothing there.");
    }
}
