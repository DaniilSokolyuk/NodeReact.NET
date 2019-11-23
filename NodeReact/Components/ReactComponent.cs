using System;
using System.Buffers;
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

            using (var executeEngineCode = GetEngineCodeExecute())
            {
                try
                {
                    var renderResult = await _nodeInvocationService.Invoke<RenderResult>("evalCode", executeEngineCode);
                    OutputHtml = renderResult.html;
                }
                catch (Exception ex)
                {
                    ExceptionHandler(ex, ComponentName, ContainerId);
                }
            }
        }

        private IMemoryOwner<char> GetEngineCodeExecute()
        {
            using (var writer = new ArrayPooledTextWriter())
            {
                writer.Write("var context={};");
                writer.Write("Object.assign(context, {html:");

                writer.Write(ServerOnly ? "ReactDOMServer.renderToStaticMarkup(React.createElement(" : "ReactDOMServer.renderToString(React.createElement(");
                writer.Write(ComponentName);
                writer.Write(',');
                WriterSerialziedProps(writer);
                writer.Write("))");

                writer.Write("})");

                return writer.GetMemoryOwner();
            }
        }

        private class RenderResult
        {
            public IMemoryOwner<char> html { get; set; }

        }
    }
}