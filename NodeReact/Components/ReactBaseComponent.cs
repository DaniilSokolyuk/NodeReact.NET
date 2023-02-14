using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IO;
using NodeReact.AspNetCore.ViewEngine;
using NodeReact.Utils;

namespace NodeReact.Components
{
    public abstract class ReactBaseComponent : IDisposable
    {
        protected readonly INodeInvocationService _nodeInvocationService;
        protected readonly ReactConfiguration _configuration;
        private readonly IReactIdGenerator _reactIdGenerator;
        private readonly IComponentNameInvalidator _componentNameInvalidator;

        protected ReactBaseComponent(
            ReactConfiguration configuration,
            IReactIdGenerator reactIdGenerator,
            INodeInvocationService nodeInvocationService,
            IComponentNameInvalidator componentNameInvalidator)
        {
            _configuration = configuration;
            _reactIdGenerator = reactIdGenerator;
            _componentNameInvalidator = componentNameInvalidator;
            _nodeInvocationService = nodeInvocationService;

            ExceptionHandler = _configuration.ExceptionHandler;
        }

        public async Task<RoutingContext> Render(RenderOptions options)
        {
            SerializedProps ??= _configuration.PropsSerializer.Serialize(Props);

            var httpResponseMessage = await _nodeInvocationService.Invoke<HttpResponseMessage>(
                "renderComponent",
                new object[] { ContainerId, options, SerializedProps });

            string url = null;
            if (httpResponseMessage.Headers.TryGetValues("RspUrl", out var urlHeader))
            {
                url = urlHeader.FirstOrDefault();
            }

            int? code = null;
            if (httpResponseMessage.Headers.TryGetValues("RspCode", out var codeHeader) &&
                int.TryParse(codeHeader.FirstOrDefault(), out var codeValue))
            {
                code = codeValue;
            }

            return new RoutingContext(
                url,
                code,
                streamToCopyTo => httpResponseMessage.Content.CopyToAsync(streamToCopyTo));
        }


        private string _componentName;

        /// <summary>
        /// Gets or sets the name of the component
        /// </summary>
        public string ComponentName
        {
            get => _componentName;
            set
            {
                if (!_componentNameInvalidator.IsValid(value))
                {
                    ThrowHelper.ThrowComponentInvalidNameException(value);
                }

                _componentName = value;
            }
        }

        private string _containerId;

        /// <summary>
        /// Gets or sets the unique ID for the DIV container of this component
        /// </summary>
        public string ContainerId
        {
            get => _containerId ??= _reactIdGenerator.Generate();
            set => _containerId = value;
        }

        /// <summary>
        /// Gets or sets the HTML tag the component is wrapped in
        /// </summary>
        public string ContainerTag { get; set; } = "div";

        /// <summary>
        /// Gets or sets the HTML class for the container of this component
        /// </summary>
        public string ContainerClass { get; set; }

        /// <summary>
        /// Get or sets if this components only should be rendered server side
        /// </summary>
        public bool ServerOnly { get; set; }

        private bool _clientOnly;

        /// <summary>
        /// Get or sets if this components only should be rendered client side
        /// </summary>
        public bool ClientOnly
        {
            get => !_configuration.UseServerSideRendering || _clientOnly;
            set => _clientOnly = value;
        }

        public Func<string> NonceProvider { get; set; }

        public bool BootstrapInPlace { get; set; }

        /// <summary>
        /// Sets the props for this component
        /// </summary>
        public object Props { get; set; }

        public Action<Exception, string, string> ExceptionHandler { get; set; }


        public delegate string BootstrapScriptContent(string componentId);

        /// <summary>
        /// If specified, this string will be placed in an inline &lt;script&gt; tag after window.__nrp props
        /// </summary>
        public BootstrapScriptContent BootstrapScriptContentProvider { get; set; }

        internal PropsSerialized SerializedProps { get; set; }

        private void WriterSerialziedProps(TextWriter writer)
        {
            SerializedProps ??= _configuration.PropsSerializer.Serialize(Props);
            WriteUtf8Stream(writer, SerializedProps.Stream);
        }

        private protected PooledStream OutputHtml { get; set; }

        public void WriteOutputHtmlTo(TextWriter writer)
        {
            if (ServerOnly)
            {
                WriteUtf8Stream(writer, OutputHtml.Stream);
                return;
            }

            writer.Write('<');
            writer.Write(ContainerTag);
            writer.Write(" id=\"");
            writer.Write(ContainerId);
            writer.Write('"');
            if (!string.IsNullOrEmpty(ContainerClass))
            {
                writer.Write(" class=\"");
                writer.Write(ContainerClass);
                writer.Write('"');
            }

            writer.Write('>');

            if (!ClientOnly)
            {
                WriteUtf8Stream(writer, OutputHtml?.Stream);
            }

            writer.Write("</");
            writer.Write(ContainerTag);
            writer.Write('>');

            if (BootstrapInPlace)
            {
                writer.Write("<script");
                if (NonceProvider != null)
                {
                    writer.Write(" nonce=\"");
                    writer.Write(NonceProvider());
                    writer.Write("\"");
                }

                writer.Write(">");
                writer.Write("(window.__nrp = window.__nrp || {})['");
                writer.Write(ContainerId);
                writer.Write("'] = ");
                WriterSerialziedProps(writer);
                writer.Write(';');

                if (BootstrapScriptContentProvider != null)
                {
                    writer.Write(BootstrapScriptContentProvider(ContainerId));
                }

                writer.Write("</script>");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WriteUtf8Stream(TextWriter writer, RecyclableMemoryStream stream)
        {
            if (stream?.Length == 0)
            {
                return;
            }

            stream.Position = 0;
            var textWriterBufferWriter = new TextWriterBufferWriter(writer);

            Encoding.UTF8.GetDecoder().Convert(
                stream.GetReadOnlySequence(),
                textWriterBufferWriter,
                true,
                out _,
                out _);
        }

        /// <summary>
        /// Renders the JavaScript required to initialise this component client-side. This will
        /// initialise the React component, which includes attach event handlers to the
        /// server-rendered HTML.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.IO.TextWriter" /> to which the content is written</param>
        /// <returns>JavaScript</returns>
        public void RenderJavaScript(TextWriter writer)
        {
            if (ClientOnly)
            {
                //ReactDOM.createRoot(document.getElementById("container")).render(React.createElement(HelloWorld, { name: "John" }));
                writer.Write("ReactDOM.createRoot(document.getElementById(\"");
                writer.Write(ContainerId);
                writer.Write("\")).render(React.createElement(");
                writer.Write(ComponentName);
                writer.Write(',');
                if (BootstrapInPlace)
                {
                    writer.Write("window.__nrp['");
                    writer.Write(ContainerId);
                    writer.Write("']");
                }
                else
                {
                    WriterSerialziedProps(writer);
                }

                writer.Write("))");
            }
            else
            {
                writer.Write("ReactDOM.hydrateRoot(document.getElementById(\"");
                writer.Write(ContainerId);
                writer.Write("\"), React.createElement(");
                writer.Write(ComponentName);
                writer.Write(',');
                if (BootstrapInPlace)
                {
                    writer.Write("window.__nrp['");
                    writer.Write(ContainerId);
                    writer.Write("']");
                }
                else
                {
                    WriterSerialziedProps(writer);
                }
                writer.Write("))");
            }
        }

        public virtual void Dispose()
        {
            OutputHtml?.Dispose();
            SerializedProps?.Dispose();
        }
    }
}