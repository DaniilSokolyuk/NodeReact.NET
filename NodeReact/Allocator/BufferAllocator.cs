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
        public static BufferAllocator Instance = new BufferAllocator(16 * 1024);

        public BufferAllocator(int poolSelectorThresholdInBytes)
        {
            PoolSelectorThresholdInBytes = poolSelectorThresholdInBytes;
            InitArrayPools();
        }

        private ArrayPool<byte> normalArrayPool;
        private ArrayPool<byte> largeArrayPool;

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

            ArrayPool<byte> pool = GetArrayPool(bufferSizeInBytes);
            byte[] byteArray = pool.Rent(bufferSizeInBytes);

            var buffer = new PooledBuffer<T>(byteArray, length, pool);

            return buffer;
        }


        private void InitArrayPools()
        {
            largeArrayPool = ArrayPool<byte>.Shared;
            normalArrayPool = ArrayPool<byte>.Shared;
        }

        private ArrayPool<byte> GetArrayPool(int bufferSizeInBytes)
        {
            return bufferSizeInBytes <= PoolSelectorThresholdInBytes ? normalArrayPool : largeArrayPool;
        }

        public int PoolSelectorThresholdInBytes { get; set; }
    }
}
