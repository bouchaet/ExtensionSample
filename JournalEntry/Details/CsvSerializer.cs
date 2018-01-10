using System.Text;
using JournalEntry.UseCases;

namespace JournalEntry.Adapters
{
    public class CsvSerializer : ICsvSerializer
    {
        public string ToCsv<T>(T obj)
        {
            return InternalToCsv((dynamic) obj);
        }

        private static string InternalToCsv(dynamic dynObj)
        {
            var sb = new StringBuilder();
            object obj = dynObj;
            foreach (var prop in obj.GetType().GetProperties())
            {
                if (sb.Length > 1)
                    sb.Append(",");
                sb.Append(prop.GetValue(obj));
            }
            return sb.ToString();
        }

        private static string InternalToCsv(GlEntry obj)
        {
            return $"{obj.TradeIdentifier},{obj.GlType}";
        }
    }
}