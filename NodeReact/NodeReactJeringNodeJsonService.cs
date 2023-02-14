﻿using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Jering.Javascript.NodeJS;

namespace NodeReact
{
    internal class NodeReactJeringNodeJsonService : IJsonService
    {
        private static readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
        {
            Converters =
            {
                new PropsSerializedJsonConverter()
            },
            DefaultBufferSize = 64536,
            
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNameCaseInsensitive = true
        };


        /// <inheritdoc />
        public ValueTask<T> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken = default)
        {
            return JsonSerializer.DeserializeAsync<T>(stream, _jsonSerializerOptions, cancellationToken);
        }

        /// <inheritdoc />
        public Task SerializeAsync<T>(Stream stream, T value, CancellationToken cancellationToken = default)
        {
            return JsonSerializer.SerializeAsync(stream, value, _jsonSerializerOptions, cancellationToken);
        }
    }
}
