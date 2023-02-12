﻿using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Jering.Javascript.NodeJS;
using Newtonsoft.Json;
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

        /// <summary>
        /// Sets the props for this component
        /// </summary>
        public object Props { get; set; }

        public Action<Exception, string, string> ExceptionHandler { get; set; }

        internal PropsSerialized SerializedProps { get; set; }
        protected void WriterSerialziedProps(TextWriter writer)
        {
            SerializedProps ??= _configuration.PropsSerializer.Serialize(Props);

            var stream = SerializedProps.Stream;
            stream.Position = 0;

            var textWriterBufferWriter = new TextWriterBufferWriter(writer);
            
            Encoding.UTF8.GetDecoder().Convert(
                stream.GetReadOnlySequence(), 
                textWriterBufferWriter, 
                true, 
                out _, 
                out _);
        }

        protected IMemoryOwner<char> OutputHtml { get; set; }
        public void WriteOutputHtmlTo(TextWriter writer)
        {
            if (ServerOnly)
            {
                WriteSpan(writer, OutputHtml);
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
                WriteSpan(writer, OutputHtml);
            }

            writer.Write("</");
            writer.Write(ContainerTag);
            writer.Write('>');
        }
        
        
        //TODO: because PagedBufferedTextWriter not has override for SPANS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WriteSpan(TextWriter viewWriter, IMemoryOwner<char> owner)
        {
            if (owner is PooledBuffer<char> buffer)
            {
                viewWriter.Write(buffer.Data, 0, buffer.Length);
            }
            else
            {
                viewWriter.Write(owner.Memory.Span);
            }
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
                writer.Write(ClientOnly ? "ReactDOM.createRoot(document.getElementById(\"" : "ReactDOM.hydrateRoot(document.getElementById(\"");
                writer.Write(ContainerId);
                writer.Write("\")).render(React.createElement(");
                writer.Write(ComponentName);
                writer.Write(',');
                WriterSerialziedProps(writer);
                writer.Write("))");
            }
            else
            {
                //ReactDOM.hydrateRoot(document.getElementById("container"), React.createElement(HelloWorld, { name: "John" }))
                writer.Write("ReactDOM.hydrateRoot(document.getElementById(\"");
                writer.Write(ContainerId);
                writer.Write("\"), React.createElement(");
                writer.Write(ComponentName);
                writer.Write(',');
                WriterSerialziedProps(writer);
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
