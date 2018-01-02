using JournalEntry.Adapters;
using Newtonsoft.Json;

namespace JournalEntry.Details
{
    public class NewtonSerializer : IJsonSerializer
    {
        public string ToJson<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}
