using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Http
{
    public class Lexer : IEnumerator<(string, byte[])?>
    {
        private Stream _stream;
        private (string, byte[])? _current;

        public Lexer(Stream bytes)
        {
            _current = null;
        }

        public (string, byte[])? Current => _current;

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            _stream?.Dispose();
        }

        public bool MoveNext()
        {
            return false;
        }

        public void Reset()
        {
            throw new System.NotImplementedException();
        }
    }
}