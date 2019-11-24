using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace NodeReact.Allocator
{
    /// <summary>
    /// Inspired by https://github.com/SixLabors/Core/blob/master/src/SixLabors.Core/Memory/ArrayPoolMemoryAllocator.Buffer%7BT%7D.cs
    /// </summary>
    internal class BufferAllocator
    {
        public static BufferAllocator Instance = new BufferAllocator();

        public BufferAllocator()
        {
            InitArrayPools();
        }

        private ArrayPool<byte> arrayPool;

        public IMemoryOwner<T> Allocate<T>(int length) where T : struct
        {
            int itemSizeBytes = Unsafe.SizeOf<T>();
            int bufferSizeInBytes = length * itemSizeBytes;
            if (bufferSizeInBytes < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(length),
                    $"{nameof(BufferAllocator)} can not allocate {length} elements of {typeof(T).Name}.");
            }

            ArrayPool<byte> pool = arrayPool;
            byte[] byteArray = pool.Rent(bufferSizeInBytes);

            var buffer = new PooledBuffer<T>(byteArray, length, pool);

            return buffer;
        }


        private void InitArrayPools()
        {
            arrayPool = ArrayPool<byte>.Create(128 * 1024 * 1024, 64);
        }
    }
}
