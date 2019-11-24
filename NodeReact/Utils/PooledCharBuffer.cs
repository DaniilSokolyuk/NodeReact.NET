using System;
using System.Buffers;

namespace NodeReact.Utils
{
    internal sealed class PooledCharBuffer : IMemoryOwner<char>
    {
        public PooledCharBuffer(char[] array, int length)
        {
            _array = array;
            _length = length;
        }

        public char[] _array;

        public int _length;

        public void Dispose()
        {
            var array = _array;
            if (array != null && _length > 0)
            {
                _array = null;
                ArrayPool<char>.Shared.Return(array);
            }
        }

        public Memory<char> Memory
        {
            get
            {
                var array = _array;

                return new Memory<char>(array, 0, _length);
            }
        }
    }
}