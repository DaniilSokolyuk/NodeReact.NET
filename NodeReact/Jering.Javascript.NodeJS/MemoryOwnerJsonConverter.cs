using System;
using System.Buffers;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using NodeReact.Allocator;
using NodeReact.Utils;

namespace NodeReact
{
    internal class MemoryOwnerJsonConverter : JsonConverter<IMemoryOwner<char>>
    {
        public override bool CanConvert(Type type)
        {
            return typeof(IMemoryOwner<char>).IsAssignableFrom(type);
        }

        public override IMemoryOwner<char> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var length = reader.HasValueSequence ? (int)reader.ValueSequence.Length : reader.ValueSpan.Length;    
            var buffer = BufferAllocator.Instance.AllocateChar(length);
            var writen = reader.CopyString(buffer.Memory.Span);

            if (writen != length)
            {
                var newBuffer = BufferAllocator.Instance.AllocateChar(writen);
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
