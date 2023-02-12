using System.Buffers;

namespace NodeReact.Utils
{
    internal class BufferAllocator
    {
        public static BufferAllocator Instance = new BufferAllocator();

        public BufferAllocator()
        {
            InitArrayPools();
        }

        private ArrayPool<char> arrayCharPool;

        public IMemoryOwner<char> AllocateChar(int length)
        {
            ArrayPool<char> pool = length > 8192 ? arrayCharPool : ArrayPool<char>.Shared;
            char[] charArray = pool.Rent(length);

            var buffer = new PooledBuffer<char>(charArray, length, pool);

            return buffer;
        }

        private void InitArrayPools()
        {
            arrayCharPool = ArrayPool<char>.Create(16 * 1024 * 1024, 64);
        }
    }
}
