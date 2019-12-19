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
                IMemoryOwner<byte> unescapedArray = null;

                Span<byte> utf8Unescaped = span.Length <= 256 ?
                    stackalloc byte[span.Length] :
                    (unescapedArray = BufferAllocator.Instance.AllocateByte(span.Length)).Memory.Span;

                JsonReaderHelperUnescape(span, utf8Unescaped, idx, out var written);

                utf8Unescaped = utf8Unescaped.Slice(0, written);

                var result = TranscodeHelper(utf8Unescaped);

                unescapedArray?.Dispose();

                return result;
            }

            return TranscodeHelper(span);
        }

        private static IMemoryOwner<char> TranscodeHelper(ReadOnlySpan<byte> source)
        {
            int maxCharsCount = Encoding.UTF8.GetCharCount(source);
            var buffer = BufferAllocator.Instance.AllocateChar(maxCharsCount);
            Encoding.UTF8.GetChars(source, buffer.Memory.Span);
            return buffer;
        }


        public override void Write(Utf8JsonWriter writer, IMemoryOwner<char> value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Memory.Span);
        }
    }
}
