using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NodeReact
{
    internal class PropsSerializedJsonConverter : JsonConverter<PropsSerialized>
    {
        public override bool CanConvert(Type type)
        {
            return typeof(PropsSerialized).IsAssignableFrom(type);
        }

        public override PropsSerialized Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        //TODO: https://github.com/dotnet/runtime/pull/76444 .NET 8.0 huge perf improvement
        public override void Write(Utf8JsonWriter writer, PropsSerialized value, JsonSerializerOptions options)
        {
            var span = value.Stream.GetBuffer().AsSpan().Slice(0, (int)value.Stream.Length);
            writer.WriteRawValue(span, true);
        }
    }
}
