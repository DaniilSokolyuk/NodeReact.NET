using System;
using System.Buffers;
using System.Reflection;
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

        public const byte BackSlash = (byte)'\\';

        delegate void Unescape(ReadOnlySpan<byte> source, Span<byte> destination, int idx, out int written);

        //https://github.com/dotnet/corefx/issues/35386
        private static Unescape JsonReaderHelperUnescape = (Unescape)Delegate.CreateDelegate(
            type: typeof(Unescape),
            method: typeof(Utf8JsonReader).Assembly.GetType("System.Text.Json.JsonReaderHelper")
                .GetMethod("Unescape", BindingFlags.NonPublic| BindingFlags.Static));


        public override IMemoryOwner<char> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            ReadOnlySpan<byte> span = reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan;

            int idx = span.IndexOf(BackSlash);
            if (idx != -1)
            {
                byte[] unescapedArray = null;

                Span<byte> utf8Unescaped = span.Length <= 256 ?
                    stackalloc byte[span.Length] :
                    (unescapedArray = ArrayPool<byte>.Shared.Rent(span.Length));

                JsonReaderHelperUnescape(span, utf8Unescaped, idx, out var written);

                utf8Unescaped = utf8Unescaped.Slice(0, written);

                var result = TranscodeHelper(utf8Unescaped);

                if (unescapedArray != null)
                {
                    ArrayPool<byte>.Shared.Return(unescapedArray);
                }

                return result;
            }

            return TranscodeHelper(span);
        }

        private static PooledCharBuffer TranscodeHelper(ReadOnlySpan<byte> source)
        {
            int maxCharsCount = Encoding.UTF8.GetMaxCharCount(source.Length);
            var rentedArray = ArrayPool<char>.Shared.Rent(maxCharsCount);
            var length = Encoding.UTF8.GetChars(source, rentedArray);
            return new PooledCharBuffer(rentedArray, length);
        }


        public override void Write(Utf8JsonWriter writer, IMemoryOwner<char> value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Memory.Span);
        }
    }
}
