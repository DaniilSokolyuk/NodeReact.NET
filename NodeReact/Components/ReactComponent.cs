using System;
using System.Net.Http;
using System.Threading.Tasks;
using NodeReact.Utils;

namespace NodeReact.Components
{
    /// <summary>
    /// Represents a React JavaScript component.
    /// </summary>
    public sealed class ReactComponent : ReactBaseComponent
    {
        public ReactComponent(
            ReactConfiguration configuration,
            IReactIdGenerator reactIdGenerator,
            INodeInvocationService nodeInvocationService,
            IComponentNameInvalidator componentNameInvalidator) : base(
            configuration,
            reactIdGenerator,
            nodeInvocationService,
            componentNameInvalidator)
        {
        }

        public async Task RenderHtml()
        {
            if (ClientOnly)
            {
                return;
            }

            try
            {
                SerializedProps ??= _configuration.PropsSerializer.Serialize(Props);

                var httpResponseMessage = await _nodeInvocationService.Invoke<HttpResponseMessage>(
                    "renderComponent",
                    new object[] { ContainerId, ComponentName, ServerOnly, new
                    {
                        disableStreaming = true,
                        disableBootstrapPropsInPlace = true,
                    }, SerializedProps });

                OutputHtml = new PooledStream();
                await httpResponseMessage.Content.CopyToAsync(OutputHtml.Stream);
            }
            catch (Exception ex)
            {
                ExceptionHandler(ex, ComponentName, ContainerId);
            }
        }
    }
}