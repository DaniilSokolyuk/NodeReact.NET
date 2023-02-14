using Microsoft.AspNetCore.Mvc.Rendering;

namespace NodeReact.AspNetCore.ViewEngine;

/// <summary>
/// INodeReactViewOptionsProvider provider is called on each rendering of a react component from NodeReactViewEngine.
/// </summary>
public interface INodeReactRenderOptionsProvider
{
    /// <summary>
    /// Provide node react direct streaming options.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="componentName"></param>
    /// <returns></returns>
    RenderOptions Provide(ViewContext context, string componentName);
}
