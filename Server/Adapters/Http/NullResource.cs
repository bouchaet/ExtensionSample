using Entities.Http;

namespace Server.Adapters.Http
{
    internal class NullResource : Resource
    {
        public override (int status, string content) Delete(IHttpRequest request)
            => MakeNotFound;

        public override (int status, string content) Get(IHttpRequest request)
            => MakeNotFound;

        public override (int status, string content) Post(IHttpRequest request)
            => MakeNotFound;

        public override (int status, string content) Put(IHttpRequest request)
            => MakeNotFound;

        private static (int, string) MakeNotFound => (404, "Sorry, nothing there.");
    }
}
