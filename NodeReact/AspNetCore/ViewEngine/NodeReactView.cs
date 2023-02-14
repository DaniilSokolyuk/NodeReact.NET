using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.DependencyInjection;
using NodeReact.Components;
using NodeReact.Utils;

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

        var optionsProvider = httpContext.RequestServices.GetService<INodeReactRenderOptionsProvider>();
        var options = optionsProvider?.Provide(context, _componentName) ?? new RenderOptions();
        options.Location ??= request.Path.ToString() + request.QueryString;
        options.ComponentName ??= _componentName;

        var scopedContext = httpContext.RequestServices.GetRequiredService<IReactScopedContext>();

        var reactComponent = scopedContext.CreateComponent<ReactComponent>(componentName: options.ComponentName);
        reactComponent.Props = context.ViewData.Model;

        var routingContext = await reactComponent.Render(options);
        if (routingContext == null)
        {
            await options.RoutingHandler(response, null);
        }
        
        if (routingContext?.StatusCode != null || routingContext?.Url != null)
        {
            // Use provided contextHandler
            if (options.RoutingHandler != null)
            {
                await options.RoutingHandler(response, routingContext);
            }
            // Handle routing context internally
            else
            {
                var statusCode = routingContext.StatusCode ?? 302;

                // 300-399
                if (statusCode >= 300 && statusCode < 400)
                {
                    if (!string.IsNullOrEmpty(routingContext.Url))
                    {
                        if (statusCode == 301)
                        {
                            response.Redirect(routingContext.Url, true);
                        }
                        else // 302 and all others
                        {
                            response.Redirect(routingContext.Url);
                        }
                    }
                    else
                    {
                        throw new NodeReactException("Router requested redirect but no url provided.");
                    }
                }
                else
                {
                    response.StatusCode = statusCode;
                }
            }
        }
        
        await routingContext!.CopyToStream(response.Body);
    }

    public string Path { get; }
}