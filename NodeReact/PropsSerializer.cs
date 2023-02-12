using System;
using System.Buffers;
using System.IO;
using System.Text;
using System.Text.Json;
using NodeReact.Utils;
using NewtonsoftJson = Newtonsoft.Json;
using NewtonsoftJsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace NodeReact;

internal interface IPropsSerializer
{
    PropsSerialized Serialize(object props);
}

internal class NewtonsoftJsonPropsSerializer : IPropsSerializer
{
    private readonly NewtonsoftJsonSerializer _jsonSerializer;

    public NewtonsoftJsonPropsSerializer(NewtonsoftJsonSerializer jsonSerializer)
    {
        _jsonSerializer = jsonSerializer;
    }
    
    public PropsSerialized Serialize(object props)
    {
        var pooledStream = new PooledStream();
        using var streamWriter = new StreamWriter(pooledStream.Stream, Encoding.UTF8, -1, true);
        using var jsonWriter = new NewtonsoftJson.JsonTextWriter(streamWriter);
        jsonWriter.CloseOutput = false;
        jsonWriter.AutoCompleteOnClose = false;
        jsonWriter.ArrayPool = JsonArrayPool<char>.Instance;
        _jsonSerializer.Serialize(jsonWriter, props);

        return new PropsSerialized(pooledStream);
    }
    
    private class JsonArrayPool<T> : NewtonsoftJson.IArrayPool<T>
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

internal class SystemTextJsonPropsSerializer : IPropsSerializer
{
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public SystemTextJsonPropsSerializer(JsonSerializerOptions jsonSerializerOptions)
    {
        _jsonSerializerOptions = jsonSerializerOptions;
    }

    public PropsSerialized Serialize(object props)
    { 
        var pooledStream = new PooledStream();
        JsonSerializer.Serialize(new Utf8JsonWriter((IBufferWriter<byte>)pooledStream.Stream), props, _jsonSerializerOptions);

        return new PropsSerialized(pooledStream);
    }
}