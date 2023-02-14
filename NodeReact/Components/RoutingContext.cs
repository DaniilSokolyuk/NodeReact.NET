using System;
using System.IO;
using System.Threading.Tasks;

namespace NodeReact.Components;
public class RoutingContext
{
    public RoutingContext(string url, int? statusCode, Func<Stream, Task> copyToStream)
    {
        StatusCode = statusCode;
        Url = url;
        CopyToStream = copyToStream;
    }

    public Func<Stream, Task> CopyToStream { get; } 

    /// <summary>
    /// HTTP Status Code.
    /// If present signifies that the given status code should be returned by server.
    /// </summary>
    public int? StatusCode { get; }

    /// <summary>
    /// URL to redirect to.
    /// If included this signals that React Router determined a redirect should happen.
    /// </summary>
    public string Url { get; }
}