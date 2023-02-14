using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NodeReact.Components;

namespace NodeReact.AspNetCore.ViewEngine;


/// <summary>
/// Node react stream options, more information here https://beta.reactjs.org/reference/react-dom/server/renderToPipeableStream
/// </summary>
public class RenderOptions
{
    /// <summary>
    /// Replace default location (request.Path.ToString() + request.QueryString).
    /// </summary>
    public string Location { get; set; }

    /// <summary>
    /// Replace component name.
    /// </summary>
    public string ComponentName { get; set; }
    
    /// <summary>
    /// Render on server only, disables almost all features, useful for static generations.
    /// </summary>
    public bool ServerOnly { get; set; }
    
    /// <summary>
    /// Disable streaming and return all data from 'onAllReady' without waiting, useful for static generations and search crawlers.
    /// </summary>
    public bool DisableStreaming { get; set; }
    
    /// <summary>
    /// Disable boostrap props in window.__nrp.
    /// </summary>
    public bool DisableBootstrapPropsInPlace { get; set; }

    /// <summary>
    /// If specified, this string will be placed in an inline &lt;script&gt; tag after window.__nrp props.
    /// You should hydrateRoot here or in BootstrapScripts/BootstrapModules.
    /// https://github.com/reactwg/react-18/discussions/114
    /// </summary>
    public string BootstrapScriptContent { get; set; } = "window.__nrpBoot ? __nrpBoot() : (window.__nrpReady = true)";
    
    /// <summary>
    /// An array of string URLs for the &lt;script&gt; tags to emit on the page. Use this to include the &lt;script&gt;
    /// that calls hydrateRoot. Omit it if you don’t want to run React on the client at all.
    /// You should hydrateRoot here or in BootstrapScriptContent.
    /// https://github.com/reactwg/react-18/discussions/114
    /// </summary>
    public string[] BootstrapScripts { get; set; }
    
    /// <summary>
    /// Like bootstrapScripts, but emits &lt;script type=&quot;module&quot;&gt; instead
    /// </summary>
    public string[] BootstrapModules { get; set; }
    
    /// <summary>
    /// A string with the root namespace URI for the stream. Defaults to regular HTML.
    /// Pass &apos;http://www.w3.org/2000/svg&apos; for SVG or &apos;http://www.w3.org/1998/Math/MathML&apos; for MathML.
    /// </summary>
    public string NamespaceURI { get; set; }
    
    /// <summary>
    /// A nonce string to allow scripts for script-src Content-Security-Policy. If not provided ScriptNonceProvider
    /// from ReactConfiguration will be used.
    /// </summary>
    public string Nonce { get; set; }
    
    /// <summary>
    /// The number of bytes in a chunk
    /// https://github.com/facebook/react/blob/14c2be8dac2d5482fda8a0906a31d239df8551fc/packages/react-server/src/ReactFizzServer.js#L210-L225
    /// </summary>
    public int? ProgressiveChunkSize { get; set; }

    public  Func<HttpResponse, RoutingContext, Task> RoutingHandler { get; set; }
}