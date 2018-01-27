using System;
using System.Linq;
using System.Reflection;

namespace Server.Adapters.Http
{
    internal class HttpStatusCode
    {
        public static readonly HttpStatusCode Ok = new HttpStatusCode(200, "OK");
        public static readonly HttpStatusCode BadRequest = new HttpStatusCode(400, "Bad Request");
        public static readonly HttpStatusCode NotFound = new HttpStatusCode(404, "Not Found");
        public static readonly HttpStatusCode ServerError = new HttpStatusCode(500, "Server Error");

        public int Code { get; private set; }
        public string Description { get; private set; }

        private HttpStatusCode(int status, string description)
        {
            Code = status;
            Description = description;
        }

        public static HttpStatusCode From(int status)
        {
            var statusCode = typeof(HttpStatusCode)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.FieldType == typeof(HttpStatusCode))
                .Select(f => (HttpStatusCode) f.GetValue(null))
                .FirstOrDefault(x => x.Code == status);

            if (statusCode == null)
                throw new ArgumentException(
                    nameof(status),
                    $"Status {status} is not supported.");

            return statusCode;
        }
    }
}
