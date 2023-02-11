using System.Buffers;

namespace NodeReact.Allocator
{
    internal class BufferAllocator
    {
        public static BufferAllocator Instance = new BufferAllocator();

        public BufferAllocator()
        {
            InitArrayPools();
        }

        private ArrayPool<char> arrayCharPool;
        private ArrayPool<byte> arrayBytePool;

        public IMemoryOwner<char> AllocateChar(int length)
        {
            ArrayPool<char> pool = length > 8192 ? arrayCharPool : ArrayPool<char>.Shared;
            char[] charArray = pool.Rent(length);

            var buffer = new PooledBuffer<char>(charArray, length, pool);

            return buffer;
        }

        public IMemoryOwner<byte> AllocateByte(int length)
        {
            ArrayPool<byte> pool = length > 8192 ? arrayBytePool : ArrayPool<byte>.Shared;
            byte[] charArray = pool.Rent(length);

            var buffer = new PooledBuffer<byte>(charArray, length, pool);

            return buffer;
        }


        private void InitArrayPools()
        {
            arrayBytePool = ArrayPool<byte>.Create(128 * 1024 * 1024, 64);
            arrayCharPool = ArrayPool<char>.Create(128 * 1024 * 1024, 64);
        }
    }
}
