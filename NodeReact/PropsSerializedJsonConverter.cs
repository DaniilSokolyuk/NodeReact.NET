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
        
        public override void Write(Utf8JsonWriter writer, PropsSerialized value, JsonSerializerOptions options)
        {
            value.Stream.Position = 0;
            foreach (var seq in value.Stream.GetReadOnlySequence())
            {
                writer.WriteRawValue(seq.Span, true);
            }
        }
    }
}
