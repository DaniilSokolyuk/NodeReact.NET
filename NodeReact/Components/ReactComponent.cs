using System;
using System.IO;
using System.Text;
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

                var renderResult = await _nodeInvocationService.Invoke<Stream>(
                    "renderComponent",
                    new object[] { ContainerId, ComponentName, ServerOnly, SerializedProps });

                OutputHtml = new PooledStream();
                await renderResult.CopyToAsync(OutputHtml.Stream);
            }
            catch (Exception ex)
            {
                ExceptionHandler(ex, ComponentName, ContainerId);
            }
        }
    }
}