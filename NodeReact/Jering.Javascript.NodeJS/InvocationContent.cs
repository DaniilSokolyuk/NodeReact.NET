﻿using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using Jering.Javascript.NodeJS;

namespace NodeReact
{
    public class InvocationContent : HttpContent
    {
        private static readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
        {
            Converters = { new MemoryOwnerJsonConverter() },
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            DefaultBufferSize = 64536,

            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            IgnoreNullValues = true,
        };

        // Arbitrary boundary
        internal static readonly byte[] BOUNDARY_BYTES = Encoding.UTF8.GetBytes("--Uiw6+hXl3k+5ia0cUYGhjA==");

        private static readonly MediaTypeHeaderValue _multipartContentType = new MediaTypeHeaderValue("multipart/mixed");
        private readonly NodeInvocationRequest _invocationRequest;

        /// <summary>
        /// Creates an <see cref="InvocationContent"/> instance.
        /// </summary>
        /// <param name="jsonService">The service for serializing data to JSON.</param>
        /// <param name="invocationRequest">The invocation request to transmit over Http.</param>
        public InvocationContent(NodeInvocationRequest invocationRequest)
        {
            _invocationRequest = invocationRequest;

            if (invocationRequest.ModuleSourceType == ModuleSourceType.Stream)
            {
                Headers.ContentType = _multipartContentType;
            }
        }

        /// <summary>
        /// Serialize the HTTP content to a stream as an asynchronous operation.
        /// </summary>
        /// <param name="stream">The target stream.</param>
        /// <param name="context">Information about the transport (channel binding token, for example). This parameter may be null.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {

            await JsonSerializer.SerializeAsync(stream, _invocationRequest, jsonSerializerOptions).ConfigureAwait(false);

            // TODO Stream writer allocates both a char[] and a byte[] for buffering, it is slower than just serializing to string and writing the string to the stream
            // (at least for small-average size payloads). Support for ArrayPool buffers is coming - https://github.com/dotnet/corefx/issues/23874, might need to target
            // netcoreapp2.1
            // using (var streamWriter = new StreamWriter(stream, UTF8NoBOM, 256, true))
            // using (var jsonWriter = new JsonTextWriter(streamWriter))
            // {
            //     _jsonService.Serialize(jsonWriter, _invocationRequest);
            // };

            if (_invocationRequest.ModuleSourceType == ModuleSourceType.Stream)
            {
                await stream.WriteAsync(BOUNDARY_BYTES, 0, BOUNDARY_BYTES.Length).ConfigureAwait(false);
                await _invocationRequest.ModuleStreamSource.CopyToAsync(stream).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Determines whether the HTTP content has a valid length in bytes.
        /// </summary>
        /// <param name="length">The length in bytes of the HTTP content.</param>
        /// <returns>true if length is a valid length; otherwise, false.</returns>
        protected override bool TryComputeLength(out long length)
        {
            length = -1;

            // Can't determine length
            return false;
        }
    }
}