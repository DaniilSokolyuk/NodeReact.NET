﻿using System;
using System.Buffers;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Jering.IocServices.System.Net.Http;
using Jering.Javascript.NodeJS;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
using NodeReact.Utils;

namespace NodeReact
{
    /// <summary>
    /// <para>An implementation of <see cref="OutOfProcessNodeJSService"/> that uses Http for inter-process communication.</para>
    /// <para>The NodeJS child process starts a Http server on an arbitrary port (unless otherwise specified
    /// using <see cref="NodeJSProcessOptions.Port"/>) and receives invocation requests as Http requests.</para>
    /// </summary>
    public class HttpNodeJSService : OutOfProcessNodeJSService
    {
        private static readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
        {
            Converters = { new CustomSerializerDeserializer() },
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            DefaultBufferSize = 64536
        };

        internal const string SERVER_SCRIPT_NAME = "HttpServer.js";

        private readonly IHttpClientService _httpClientService;

        private bool _disposed;
        internal Uri Endpoint;

        /// <summary>
        /// Creates a <see cref="HttpNodeJSService"/> instance.
        /// </summary>
        /// <param name="outOfProcessNodeJSServiceOptionsAccessor"></param>
        /// <param name="httpContentFactory"></param>
        /// <param name="embeddedResourcesService"></param>
        /// <param name="httpClientService"></param>
        /// <param name="jsonService"></param>
        /// <param name="nodeJSProcessFactory"></param>
        /// <param name="loggerFactory"></param>
        public HttpNodeJSService(IOptions<OutOfProcessNodeJSServiceOptions> outOfProcessNodeJSServiceOptionsAccessor,
            IEmbeddedResourcesService embeddedResourcesService,
            IHttpClientService httpClientService,
            INodeJSProcessFactory nodeJSProcessFactory,
            ILoggerFactory loggerFactory) :
            base(nodeJSProcessFactory,
                loggerFactory.CreateLogger(typeof(HttpNodeJSService)),
                outOfProcessNodeJSServiceOptionsAccessor,
                embeddedResourcesService,
                typeof(Jering.Javascript.NodeJS.HttpNodeJSService).GetTypeInfo().Assembly,
                SERVER_SCRIPT_NAME)
        {
            
            _httpClientService = httpClientService;
        }

        /// <inheritdoc />
        protected override async Task<(bool, T)> TryInvokeAsync<T>(InvocationRequest invocationRequest, CancellationToken cancellationToken)
        {
            using (HttpContent httpContent = new InvocationContent(new NodeInvocationRequest(invocationRequest)))
            using (var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, Endpoint))
            {
                httpRequestMessage.Content = httpContent;

                // Some notes on disposal:
                // HttpResponseMessage.Dispose simply calls Dispose on HttpResponseMessage.Content. By default, HttpResponseMessage.Content is a StreamContent instance that has an underlying 
                // NetworkStream instance that should be disposed. When HttpResponseMessage.Content.ReadAsStreamAsync is called, the NetworkStream is wrapped in a read-only delegating stream
                // and returned. In most cases below, StreamReader is used to read the read-only stream, upon disposal of the StreamReader, the underlying stream and thus the NetworkStream
                // are disposed. If HttpStatusCode is NotFound or an exception is thrown, we manually call HttpResponseMessage.Dispose. If we return the stream, we pass on the responsibility 
                // for disposing it to the caller.
                HttpResponseMessage httpResponseMessage = null;
                try
                {
                    httpResponseMessage = await _httpClientService.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                    if (httpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                    {
                        httpResponseMessage.Dispose();
                        return (false, default);
                    }

                    if (httpResponseMessage.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        using (Stream stream = await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false))
                        {
                            InvocationError invocationError = await JsonSerializer
                                .DeserializeAsync<InvocationError>(stream, jsonSerializerOptions, cancellationToken).ConfigureAwait(false);
                            throw new InvocationException(invocationError.ErrorMessage, invocationError.ErrorStack);
                        }
                    }

                    if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                    {
                        if (typeof(T) == typeof(Stream))
                        {
                            Stream stream = await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
                            return (true, (T)(object)stream);
                        }

                        if (typeof(T) == typeof(string))
                        {
                            string str = await httpResponseMessage.Content.ReadAsStringAsync();
                            return (true, (T)(object)str);
                        }

                        using (var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false))
                        {
                            var result = await JsonSerializer
                                .DeserializeAsync<T>(contentStream, jsonSerializerOptions, cancellationToken).ConfigureAwait(false);
                            return (true, result);
                        }
                    }
                }
                catch
                {
                    httpResponseMessage?.Dispose();

                    throw;
                }

                throw new InvocationException(string.Format("InvocationException_HttpNodeJSService_UnexpectedStatusCode", httpResponseMessage.StatusCode));
            }
        }

        /// <inheritdoc />
        protected override void OnConnectionEstablishedMessageReceived(string connectionEstablishedMessage)
        {
            // Start after message start and "IP - "
            int startIndex = CONNECTION_ESTABLISHED_MESSAGE_START.Length + 5;
            var stringBuilder = new StringBuilder("http://");

            for (int i = startIndex; i < connectionEstablishedMessage.Length; i++)
            {
                char currentChar = connectionEstablishedMessage[i];

                if (currentChar == ':')
                {
                    // ::1
                    stringBuilder.Append("[::1]");
                    i += 2;
                }
                else if (currentChar == ' ')
                {
                    stringBuilder.Append(':');

                    // Skip over "Port - "
                    i += 7;
                }
                else if (currentChar == ']')
                {
                    Endpoint = new Uri(stringBuilder.ToString());
                    return;
                }
                else
                {
                    stringBuilder.Append(currentChar);
                }
            }
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            base.Dispose(disposing);

            _disposed = true;
        }
    }
}