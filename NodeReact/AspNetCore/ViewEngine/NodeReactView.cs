using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.DependencyInjection;
using NodeReact.Components;

namespace NodeReact.AspNetCore.ViewEngine;

public class NodeReactView : IView
{
    private readonly string _componentName;

    public NodeReactView(string componentName)
    {
        _componentName = componentName;
    }

    public async Task RenderAsync(ViewContext context)
    {
        var httpContext = context.HttpContext;
        var request = httpContext.Request;
        var response = httpContext.Response;

        var optionsProvider = httpContext.RequestServices.GetService<INodeReactViewOptionsProvider>();
        var options = optionsProvider?.Provide(context, _componentName) ?? new NodeReactViewOptions();

        var scopedContext = httpContext.RequestServices.GetRequiredService<IReactScopedContext>();

        var reactComponent = scopedContext.CreateComponent<ReactRouterComponent>(componentName: options.ComponentName ?? _componentName);
        reactComponent.Props = context.ViewData.Model;
        reactComponent.Path = options.Location ?? request.Path.ToString() + request.QueryString;

        await reactComponent.RenderToStream(response.Body);
    }

    public string Path { get; }
}