using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Jering.Javascript.NodeJS;

namespace NodeReact.Jering.Javascript.NodeJS
{
    internal class CustomJsonService : IJsonService
    {
        private static readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
        {
            Converters = { new MemoryOwnerJsonConverter()},
            DefaultBufferSize = 64536,

            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            IgnoreNullValues = true,
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
