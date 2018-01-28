namespace Entities.Http
{
    public abstract class Resource
    {
        public abstract (int status, string content) Get(IHttpRequest request);
        public abstract (int status, string content) Post(IHttpRequest request);
        public abstract (int status, string content) Put(IHttpRequest request);
        public abstract (int status, string content) Delete(IHttpRequest request);
    }
}
