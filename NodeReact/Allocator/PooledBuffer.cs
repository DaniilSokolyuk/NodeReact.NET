using System;
using System.Buffers;
using System.Runtime.InteropServices;

namespace NodeReact.Allocator
{
    internal sealed class PooledBuffer<T> : MemoryManager<T> where T : struct
    {
        private WeakReference<ArrayPool<byte>> _sourcePoolReference;
        private byte[] _data;
        private readonly int _length;
        private GCHandle pinHandle;

        public PooledBuffer(byte[] data, int length, ArrayPool<byte> sourcePool)
        {
            _data = data;
            _length = length;
            _sourcePoolReference = new WeakReference<ArrayPool<byte>>(sourcePool);
        }

        public override unsafe MemoryHandle Pin(int elementIndex = 0)
        {
            if (!pinHandle.IsAllocated)
            {
                pinHandle = GCHandle.Alloc(_data, GCHandleType.Pinned);
            }

            void* ptr = (void*)pinHandle.AddrOfPinnedObject();
            return new MemoryHandle(ptr, pinHandle);
        }

        /// <inheritdoc />
        public override void Unpin()
        {
            if (pinHandle.IsAllocated)
            {
                pinHandle.Free();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposing || _data is null || _sourcePoolReference is null)
            {
                return;
            }

            if (_sourcePoolReference.TryGetTarget(out ArrayPool<byte> pool))
            {
                pool.Return(_data);
            }

            _sourcePoolReference = null;
            _data = null;
        }

        public override Span<T> GetSpan() => MemoryMarshal.Cast<byte, T>(_data.AsSpan()).Slice(0, _length);

    }
}