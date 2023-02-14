using System;
using System.Threading.Tasks;
using NodeReact.AspNetCore.ViewEngine;
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
                var routingContext = await Render(new RenderOptions
                {
                    DisableStreaming = true,
                    DisableBootstrapPropsInPlace = true,
                    BootstrapScriptContent = null,
                    ComponentName = ComponentName,
                    ServerOnly = ServerOnly,
                    Nonce = NonceProvider?.Invoke(),
                });

                OutputHtml = new PooledStream();
                await routingContext.CopyToStream(OutputHtml.Stream);
            }
            catch (Exception ex)
            {
                ExceptionHandler(ex, ComponentName, ContainerId);
            }
        }
    }
}