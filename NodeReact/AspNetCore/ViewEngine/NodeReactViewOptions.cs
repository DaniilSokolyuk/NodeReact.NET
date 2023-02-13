namespace NodeReact.AspNetCore.ViewEngine;


/// <summary>
/// Node react stream options, more information here https://beta.reactjs.org/reference/react-dom/server/renderToPipeableStream
/// </summary>
public class NodeReactViewOptions
{
    /// <summary>
    /// Disable streaming and return data from 'onAllReady', useful for static generations and search crawlers.
    /// </summary>
    public bool DisableStreaming { get; set; }
    
    /// <summary>
    /// Replace default location (request.Path.ToString() + request.QueryString).
    /// </summary>
    public string Location { get; set; }

    /// <summary>
    /// Replace component name.
    /// </summary>
    public string ComponentName { get; set; }
    
    /// <summary>
    ///  If specified, this string will be placed in an inline &lt;script&gt; tag.
    /// </summary>
    public string BootstrapScriptContent { get; set; }
    
    /// <summary>
    /// An array of string URLs for the &lt;script&gt; tags to emit on the page. Use this to include the &lt;script&gt;
    /// that calls hydrateRoot. Omit it if you don’t want to run React on the client at all
    /// </summary>
    public string BootstrapScripts { get; set; }
    
    /// <summary>
    /// Like bootstrapScripts, but emits &lt;script type=&quot;module&quot;&gt; instead
    /// </summary>
    public string BootstrapModules { get; set; }
    
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
}