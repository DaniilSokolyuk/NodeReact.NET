using Microsoft.AspNetCore.Mvc.Rendering;

namespace NodeReact.AspNetCore.ViewEngine;

/// <summary>
/// INodeReactViewOptionsProvider provider is called on each rendering of a react component from NodeReactViewEngine.
/// </summary>
public interface INodeReactViewOptionsProvider
{
    /// <summary>
    /// Provide node react direct streaming options.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="componentName"></param>
    /// <returns></returns>
    NodeReactViewOptions Provide(ViewContext context, string componentName);
}
