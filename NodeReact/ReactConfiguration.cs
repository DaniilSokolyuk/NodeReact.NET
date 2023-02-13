using System;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Jering.Javascript.NodeJS;
using Newtonsoft.Json;
using NewtonsoftJsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace NodeReact
{
    /// <summary>
    /// NodeReact configuration.
    /// </summary>
    public class ReactConfiguration
    {
        public ReactConfiguration()
        {
            ConfigureSystemTextJsonPropsSerializer(_ => {});
        }
        
        internal readonly IList<string> ScriptFilesWithoutTransform = new List<string>();
        internal Action<NodeJSProcessOptions> ConfigureNodeJSProcessOptionsAction;
        internal Action<OutOfProcessNodeJSServiceOptions> ConfigureOutOfProcessNodeJSServiceOptionsAction;
        internal Action<HttpNodeJSServiceOptions> ConfigureHttpNodeJSServiceOptionsAction;

        internal IPropsSerializer PropsSerializer { get; set; }

        public ReactConfiguration AddScriptWithoutTransform(string script)
        {
            ScriptFilesWithoutTransform.Add(script);
            return this;
        }

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
        /// File watcher debounce time in milliseconds. Defaults to 10.
        /// </summary>
        public int FileWatcherDebounceMs { get; set; } = 10;

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

        /// <summary>
        /// Set Newtonsoft.Json serializer for React props.
        /// </summary>
        /// <param name="configureJsonSerializerSettings"></param>
        public void ConfigureNewtonsoftJsonPropsSerializer(Action<JsonSerializerSettings> configureJsonSerializerSettings)
        {
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                StringEscapeHandling = StringEscapeHandling.EscapeHtml
            };
            
            configureJsonSerializerSettings(jsonSerializerSettings);
            PropsSerializer = new NewtonsoftJsonPropsSerializer(NewtonsoftJsonSerializer.Create(jsonSerializerSettings));
        }
        
        /// <summary>
        /// Set System.Text.Json serializer for React props.
        /// </summary>
        /// <param name="configureJsonSerializerOptions"></param>
        public void ConfigureSystemTextJsonPropsSerializer(Action<JsonSerializerOptions> configureJsonSerializerOptions)
        {
            var jsonSerializerOptions = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                PropertyNameCaseInsensitive = true,
            };
            
            configureJsonSerializerOptions(jsonSerializerOptions);  
            PropsSerializer = new SystemTextJsonPropsSerializer(jsonSerializerOptions);
        }
        
        public void ConfigureNodeJSProcess(Action<NodeJSProcessOptions> configureNodeJSProcessOptions)
        {
            ConfigureNodeJSProcessOptionsAction = configureNodeJSProcessOptions;
        }
        
        public void ConfigureOutOfProcessNodeJSService(Action<OutOfProcessNodeJSServiceOptions> configureOutOfProcessNodeJSServiceOptions)
        {
            ConfigureOutOfProcessNodeJSServiceOptionsAction = configureOutOfProcessNodeJSServiceOptions;
        }
        
        public void ConfigureHttpNodeJSService(Action<HttpNodeJSServiceOptions> configureHttpNodeJSServiceOptions)
        {
            ConfigureHttpNodeJSServiceOptionsAction = configureHttpNodeJSServiceOptions;
        }
    }
}
