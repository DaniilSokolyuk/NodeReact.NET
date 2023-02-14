using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using NodeReact.AspNetCore.ViewEngine;
using NodeReact.Utils;

namespace NodeReact.Components
{
    public sealed class ReactRouterComponent : ReactBaseComponent
    {
        public ReactRouterComponent(
            ReactConfiguration configuration,
            IReactIdGenerator reactIdGenerator,
            INodeInvocationService _nodeInvocationService,
            IComponentNameInvalidator componentNameInvalidator) : base(
            configuration,
            reactIdGenerator,
            _nodeInvocationService,
            componentNameInvalidator)
        {
        }

        public string Path { get; set; }
        
        public async Task<RoutingContext> RenderRouterWithContext()
        {
            if (ClientOnly)
            {
                return new RoutingContext();
            }

            try
            {
                SerializedProps ??= _configuration.PropsSerializer.Serialize(Props);

                var httpResponseMessage = await _nodeInvocationService.Invoke<HttpResponseMessage>(
                    "renderRouter",
                    new object[] { ContainerId, ComponentName, ServerOnly, SerializedProps, new
                    {
                        disableStreaming = false,
                        disableBootstrapPropsInPlace = false,
                    }, Path });

                OutputHtml = new PooledStream();
                await httpResponseMessage.Content.CopyToAsync(OutputHtml.Stream);

                return new RoutingContext();
            }
            catch (Exception ex)
            {
                ExceptionHandler(ex, ComponentName, ContainerId);
                return new RoutingContext();;
            }
        }
        
        public async Task<RoutingContext> RenderToStream(Stream stream, NodeReactViewOptions options)
        {
            try
            {
                SerializedProps ??= _configuration.PropsSerializer.Serialize(Props);

                var httpResponseMessage = await _nodeInvocationService.Invoke<HttpResponseMessage>(
                    "renderComponent",
                    new object[] { ContainerId, ComponentName, ServerOnly, SerializedProps, new
                    {
                        disableStreaming = options.DisableStreaming,
                        disableBootstrapPropsInPlace = options.DisableBootstrapPropsInPlace,
                        bootstrapScriptContent = options.BootstrapScriptContent,
                    }, Path });

                await httpResponseMessage.Content.CopyToAsync(stream);

                return new RoutingContext();
            }
            catch (Exception ex)
            {
                ExceptionHandler(ex, ComponentName, ContainerId);
                return new RoutingContext();;
            }
        }
    }

    public class RoutingContext
    {
        /// <summary>
        /// HTTP Status Code.
        /// If present signifies that the given status code should be returned by server.
        /// </summary>
        public int? StatusCode { get; set; }

        /// <summary>
        /// URL to redirect to.
        /// If included this signals that React Router determined a redirect should happen.
        /// </summary>
        public string Url { get; set; }
    }
}
