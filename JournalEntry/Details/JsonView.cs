using System.IO;
using JournalEntry.Adapters;

namespace JournalEntry.Details
{
    public class JsonView : IView
    {
        private readonly StreamWriter _writer;
        private readonly object _syncRoot = new object();
        private volatile bool _any;

        public JsonView(StreamWriter writer)
        {
            _writer = writer;
            _writer?.Write("[");
            _any = false;
        }

        public JsonView(string filename)
            : this(new StreamWriter(filename, false))
        {
        }

        ~JsonView()
        {
            try
            {
                _writer?.Write("]");
                _writer?.Flush();
                _writer?.Close();
            }
            catch (System.ObjectDisposedException)
            {
            }
        }

        public void RenderJson(string obj)
        {
            lock (_syncRoot)
            {
                if (_any)
                    _writer.Write(",");
                _writer.Write(obj);
                _any = true;
            }
            _writer.Flush();
        }

        public void RenderCsv(string obj)
        {
        }
    }
}