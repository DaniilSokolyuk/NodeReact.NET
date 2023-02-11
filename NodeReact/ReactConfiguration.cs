using System;
using System.Collections.Generic;
using Jering.Javascript.NodeJS;
using Newtonsoft.Json;

namespace NodeReact
{
    /// <summary>
    /// Site-wide configuration for ReactJS.NET
    /// </summary>
    public class ReactConfiguration
    {
        public ReactConfiguration()
        {
            SetJsonSerializerSettings(new JsonSerializerSettings
            {
                StringEscapeHandling = StringEscapeHandling.EscapeHtml
            });
        }

        /// <summary>
        /// All the scripts that have been added to this configuration 
        /// </summary>
        internal readonly IList<string> ScriptFilesWithoutTransform = new List<string>();

        public ReactConfiguration AddScriptWithoutTransform(string script)
        {
            ScriptFilesWithoutTransform.Add(script);
            return this;
        }

        internal JsonSerializerSettings JsonSerializerSettings;

        internal JsonSerializer Serializer;

        public ReactConfiguration SetJsonSerializerSettings(JsonSerializerSettings settings)
        {
            JsonSerializerSettings = settings;
            Serializer = JsonSerializer.Create(JsonSerializerSettings);
            return this;
        }

        /// <summary>
        /// ChakraCore engine settings
        /// </summary>
        public Action<NodeJSProcessOptions> ConfigureNodeJSProcessOptions { get; set; }

        public Action<OutOfProcessNodeJSServiceOptions> ConfigureOutOfProcessNodeJSServiceOptions { get; set; }

        /// <summary>
        /// Gets or sets the number of max engines. 
        /// Defaults  <c>Math.Max(Environment.ProcessorCount - 1, 2)</c>.
        /// </summary>
        public int EnginesCount { get; set; } = Math.Max(Environment.ProcessorCount - 1, 1);

        /// <summary>
        /// Gets or sets whether to use the debug version of React. This is slower, but gives
        /// useful debugging tips.
        /// </summary>
        public bool UseDebugReact { get; set; }

        /// <summary>
        /// Gets or sets whether server-side rendering is enabled.
        /// </summary>
        public bool UseServerSideRendering { get; set; } = true;

        /// <summary>
        /// A provider that returns a nonce to be used on any script tags on the page. 
        /// This value must match the nonce used in the Content Security Policy header on the response.
        /// </summary>
        public Func<string> ScriptNonceProvider { get; set; }

        /// <summary>
        /// Handle an exception caught during server-render of a component.
        /// If unset, unhandled exceptions will be thrown for all component renders.
        /// </summary>
        public Action<Exception, string, string> ExceptionHandler { get; set; } = (ex, ComponentName, ContainerId) =>
            throw new Exception(string.Format(
                "Error while rendering \"{0}\" to \"{2}\": {1}",
                ComponentName,
                ex.Message,
                ContainerId
            ));
    }
}
