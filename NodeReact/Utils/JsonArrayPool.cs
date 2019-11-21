using System;
using System.Buffers;
using Newtonsoft.Json;

namespace NodeReact.Utils
{
    internal class JsonArrayPool<T> : IArrayPool<T>
    {
        public static JsonArrayPool<T> Instance = new JsonArrayPool<T>();

        private readonly ArrayPool<T> _inner;

        public JsonArrayPool()
        {
            _inner = ArrayPool<T>.Shared;
        }

        public T[] Rent(int minimumLength)
        {
            return _inner.Rent(minimumLength);
        }

        public void Return(T[] array)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            _inner.Return(array);
        }
    }
}
