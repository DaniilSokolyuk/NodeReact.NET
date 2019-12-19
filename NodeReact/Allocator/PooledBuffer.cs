using System;
using System.Buffers;

namespace NodeReact.Allocator
{
    internal sealed class PooledBuffer<T> : IMemoryOwner<T> where T : struct
    {
        private WeakReference<ArrayPool<T>> _sourcePoolReference;

        public T[] Data { get; private set; }

        public int Length { get; }

        public PooledBuffer(T[] data, int length, ArrayPool<T> sourcePool)
        {
            Data = data;
            Length = length;
            _sourcePoolReference = new WeakReference<ArrayPool<T>>(sourcePool);
        }

        public void Dispose()
        {
            if (Data is null || _sourcePoolReference is null)
            {
                return;
            }

            if (_sourcePoolReference.TryGetTarget(out ArrayPool<T> pool))
            {
                pool.Return(Data);
            }

            _sourcePoolReference = null;
            Data = null;
        }

        public Memory<T> Memory => new Memory<T>(Data, 0, Length);
    }
}