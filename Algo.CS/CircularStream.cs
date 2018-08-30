using System;
using System.IO;

namespace Algo
    public class CircularStream : Stream
    {
        private readonly bool _canRead;
        private readonly bool _canSeek;
        private readonly bool _canWrite;
        private int _length;
        private byte[] _store;
        private long _tail;
        private long _head;
        private long _bytesRead;

        public CircularStream(int capacity = 4096)
        {
            if (!CapacityAllowed(capacity))
                capacity = 4096;

            _store = new byte[capacity];
            _canSeek = false;
            _canWrite = true;
            _canRead = true;
            _head = -1;
            _tail = 0;
            _length = capacity;
            _bytesRead = 0;
        }

        public override void Flush()
        {
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return 0;
        }

        public override void SetLength(long value)
        {
            if (!CapacityAllowed(value))
                throw new NotSupportedException();

            _length = (int)value;
            var newStore = new byte[_length];
            var count = _store.Length > _length ? _length : _store.Length;
            Array.Copy(_store, newStore, count);
            _store = newStore;
        }

        private static bool CapacityAllowed(long capacity)
        {
            return capacity <= 1048576;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (offset + count > buffer.Length)
                throw new IndexOutOfRangeException();

            var avail = _head == -1? 0 : (int)((_head - _tail) % _length);
            var max = avail > count ? count : avail;
            var tailOffset = _tail % _length;

            for (var i = 0; i < max; i++)
            {
                var next = (tailOffset + i) % _length;
                buffer[offset + i] = _store[next];
            }

            _bytesRead += max;
            _tail += max;
            return max;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            var avail = (int)(_tail % _length
                              + _length - _head % _length - 1);
            var max = avail > count ? count : avail;
            var headOffset = _head % _length + 1;

            for (var i = 0; i < max; i++)
            {
                var next = (headOffset + i) % Length;
                _store[next] = buffer[offset + i];
            }

            if (max > 0 && _head < 0)
                _head = 0;

            _head += max;
        }

        public override bool CanRead
        {
            get { return _canRead; }
        }

        public override bool CanSeek
        {
            get { return _canSeek; }
        }

        public override bool CanWrite
        {
            get { return _canWrite; }
        }

        public override long Length
        {
            get { return _length; }
        }

        public override long Position
        {
            get { return _head; }
            set { throw new NotSupportedException(); }
        }

        public long BytesRead
        {
            get { return _bytesRead; }
            set {throw new NotSupportedException();}
        }
    }
}
