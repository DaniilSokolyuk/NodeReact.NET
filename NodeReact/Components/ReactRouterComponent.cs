using System;
using System.Buffers;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
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

        public RoutingContext RoutingContext { get; private set; }

        public async Task RenderRouterWithContext()
        {
            if (ClientOnly)
            {
                return;
            }

            try
            {
                SerializedProps ??= _configuration.PropsSerializer.Serialize(Props);

                var renderResult = await _nodeInvocationService.Invoke<Stream>(
                    "renderRouter",
                    new object[] { ContainerId, ComponentName, ServerOnly, SerializedProps, Path });

                OutputHtml = new PooledStream();
                await renderResult.CopyToAsync(OutputHtml.Stream);
            }
            catch (Exception ex)
            {
                ExceptionHandler(ex, ComponentName, ContainerId);
            }
            
        }
        
     //   private IMemoryOwner<char> GetEngineCodeExecute()
     //   {
                //textWriter.Write("var context={};");
                //textWriter.Write("Object.assign(context, {html:");

               // textWriter.Write(ServerOnly ? "ReactDOMServer.renderToStaticMarkup(React.createElement(" : "ReactDOMServer.renderToString(React.createElement(");
               // textWriter.Write(ComponentName);
               // textWriter.Write(",Object.assign(");
              //  WriterSerialziedProps(textWriter);
              //  textWriter.Write(",{location:");
              //  textWriter.Write(JsonConvert.SerializeObject(Path));
              //  textWriter.Write(",context:context})))");

               // textWriter.Write("})");
    //    }
    
    
    }

    public class RoutingContext
    {
        public IMemoryOwner<char> html { get; set; }

        /// <summary>
        /// HTTP Status Code.
        /// If present signifies that the given status code should be returned by server.
        /// </summary>
        public int? status { get; set; }

        /// <summary>
        /// URL to redirect to.
        /// If included this signals that React Router determined a redirect should happen.
        /// </summary>
        public string url { get; set; }
    }
}
