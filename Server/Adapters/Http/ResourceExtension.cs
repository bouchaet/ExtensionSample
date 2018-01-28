using Entities.Http;

namespace Server.Adapters.Http
{
    internal static class ResourceExtension 
    {
        public static ResourceController ToController(this Resource resource)
        => new ResourceController(resource);
    }
}