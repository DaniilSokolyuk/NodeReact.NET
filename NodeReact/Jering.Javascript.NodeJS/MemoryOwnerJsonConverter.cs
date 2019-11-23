using System;
using System.Buffers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
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
            int maxCharsCount = Encoding.UTF8.GetMaxCharCount(reader.ValueSpan.Length);

            var rentedArray = ArrayPool<char>.Shared.Rent(maxCharsCount);

            var length = Encoding.UTF8.GetChars(reader.ValueSpan, rentedArray);

            return new PooledCharBuffer(rentedArray, length);
        }

        public override void Write(Utf8JsonWriter writer, IMemoryOwner<char> value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Memory.Span);
        }
    }
}
