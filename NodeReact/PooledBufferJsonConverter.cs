using System;
using System.Buffers;
using System.Text.Json;
using System.Text.Json.Serialization;
using NodeReact.Utils;

namespace NodeReact
{
    internal class PooledCharMemoryOwnerJsonConverter : JsonConverter<IMemoryOwner<char>>
    {
        private static MemoryPool<char> _pool = MemoryPool<char>.Shared;

        public override bool CanConvert(Type type)
        {
            return typeof(IMemoryOwner<char>).IsAssignableFrom(type);
        }

        public override IMemoryOwner<char> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var length = reader.HasValueSequence ? (int)reader.ValueSequence.Length : reader.ValueSpan.Length;    
            var buffer = _pool.Rent(length);
            var writen = reader.CopyString(buffer.Memory.Span);

            if (writen != length)
            {
                var newBuffer = _pool.Rent(writen);
                buffer.Memory.Span.Slice(0, writen).CopyTo(newBuffer.Memory.Span);
                buffer.Dispose();

                return newBuffer;
            }
            
            return buffer;
        }
        
        public override void Write(Utf8JsonWriter writer, IMemoryOwner<char> value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Memory.Span);
        }
    }
}
