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
        /// <summary>
        /// All the scripts that have been added to this configuration 
        /// </summary>
        internal readonly IList<string> ScriptFilesWithoutTransform = new List<string>();

        public ReactConfiguration AddScriptWithoutTransform(string script)
        {
            ScriptFilesWithoutTransform.Add(script);
            return this;
        }

        internal JsonSerializer Serializer = JsonSerializer.Create(
            new JsonSerializerSettings
            {
                StringEscapeHandling = StringEscapeHandling.EscapeHtml
            });

        public ReactConfiguration SetJsonSerializerSettings(JsonSerializerSettings settings)
        {
            Serializer = JsonSerializer.Create(settings);
            return this;
        }

        /// <summary>
        /// ChakraCore engine settings
        /// </summary>
        public Action<NodeJSProcessOptions> ConfigureNodeJSProcessOptions { get; set; }

        public Action<OutOfProcessNodeJSServiceOptions> ConfigureOutOfProcessNodeJSServiceOptions { get; set; }

        /// <summary>
        /// Gets or sets the number of engines to initially start when a pool is created. 
        /// Defaults to <c>Math.Max(Environment.ProcessorCount, 4)</c>.
        /// </summary>
        public int StartEngines { get; set; } = Math.Max(Environment.ProcessorCount, 4);

        /// <summary>
        /// Gets or sets the number of max engines. 
        /// Defaults  <c>Math.Max(Environment.ProcessorCount * 2, 8)</c>.
        /// </summary>
        public int MaxEngines { get; set; } = Math.Max(Environment.ProcessorCount * 2, 8);

        /// <summary>
        /// Gets or sets the maximum number of times an engine can be reused before it is disposed.
        /// <c>0</c> is unlimited. Defaults to <c>100</c>.
        /// </summary>
        public int MaxUsagesPerEngine { get; set; } = 100;

        /// <summary>
        /// Gets or sets whether the built-in version of React is loaded. If <c>false</c>, you must
        /// provide your own version of React.
        /// </summary>
        public bool LoadReact { get; set; } = true;

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
