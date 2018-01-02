using System.IO;
using JournalEntry.Adapters;

namespace JournalEntry.Details
{
    public class CsvView : IView
    {
        private readonly TextWriter _writer;

        public CsvView(TextWriter writer)
        {
            _writer = writer;
            WriteHeader();
        }

        public CsvView(string filename)
            : this(new StreamWriter(filename, false))
        {
        }

        ~CsvView()
        {
            _writer?.Flush();
            _writer?.Close();
        }

        private void WriteHeader()
        {
            _writer.WriteLine("ID, TYPE");
        }

        public void RenderJson(string s)
        {
        }

        public void RenderCsv(string s)
        {
            _writer.WriteLine(s);
            _writer.Flush();
        }
    }
}