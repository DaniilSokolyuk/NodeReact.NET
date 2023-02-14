using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using NodeReact.Components;
using NodeReact.Utils;

namespace NodeReact.AspNetCore
{
    /// <summary>
    /// HTML Helpers for utilising React from an ASP.NET MVC application.
    /// </summary>
    public static class HtmlHelperExtensions
    {
        public static async Task<IHtmlContent> ReactRouterAsync<T>(
            this IHtmlHelper htmlHelper,
            string componentName,
            T props,
            string path = null,
            string htmlTag = null,
            string containerId = null,
            bool clientOnly = false,
            bool serverOnly = false,
            string containerClass = null,
            Action<HttpResponse, RoutingContext> contextHandler = null)
        {
            var response = htmlHelper.ViewContext.HttpContext.Response;
            var request = htmlHelper.ViewContext.HttpContext.Request;
            path = path ?? request.Path.ToString() + request.QueryString;

            var scopedContext = htmlHelper.ViewContext.HttpContext.RequestServices.GetRequiredService<IReactScopedContext>();

            var reactComponent = scopedContext.CreateComponent<ReactRouterComponent>(componentName: componentName);

            reactComponent.Props = props;
            reactComponent.ContainerId = containerId;
            reactComponent.ClientOnly = clientOnly;
            reactComponent.ServerOnly = serverOnly;
            reactComponent.ContainerClass = containerClass;

            if (!string.IsNullOrEmpty(htmlTag))
            {
                reactComponent.ContainerTag = htmlTag;
            }

            reactComponent.Path = path;
            
            var executionResult = await reactComponent.RenderRouterWithContext();
            
            if (executionResult?.StatusCode != null || executionResult?.Url != null)
            {
                // Use provided contextHandler
                if (contextHandler != null)
                {
                    contextHandler(response, executionResult);
                }
                // Handle routing context internally
                else
                {
                    var statusCode = executionResult.StatusCode ?? 302;

                    // 300-399
                    if (statusCode >= 300 && statusCode < 400)
                    {
                        if (!string.IsNullOrEmpty(executionResult.Url))
                        {
                            if (statusCode == 301)
                            {
                                response.Redirect(executionResult.Url, true);
                            }
                            else // 302 and all others
                            {
                                response.Redirect(executionResult.Url);
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

            return new ActionHtmlString(writer => reactComponent.WriteOutputHtmlTo(writer));
        }

        /// <summary>
        /// Renders the specified React component
        /// </summary>
        /// <typeparam name="T">Type of the props</typeparam>
        /// <param name="htmlHelper">HTML helper</param>
        /// <param name="componentName">Name of the component</param>
        /// <param name="props">Props to initialise the component with</param>
        /// <param name="htmlTag">HTML tag to wrap the component in. Defaults to &lt;div&gt;</param>
        /// <param name="containerId">ID to use for the container HTML tag. Defaults to an auto-generated ID</param>
        /// <param name="clientOnly">Skip rendering server-side and only output client-side initialisation code. Defaults to <c>false</c></param>
        /// <param name="serverOnly">Skip rendering React specific data-attributes, container and client-side initialisation during server side rendering. Defaults to <c>false</c></param>
        /// <param name="containerClass">HTML class(es) to set on the container tag</param>
        /// <param name="exceptionHandler">A custom exception handler that will be called if a component throws during a render. Args: (Exception ex, string componentName, string containerId)</param>
        /// <param name="hydrateInPlace">If true, the component will be hydrated in place, rather you should call ReactInitJavaScript or hydrate manually. Defaults ts <c>false</c></param>
        /// <returns>The component's HTML</returns>
        public static async Task<IHtmlContent> ReactAsync<T>(
            this IHtmlHelper htmlHelper,
            string componentName,
            T props,
            string htmlTag = null,
            string containerId = null,
            bool clientOnly = false,
            bool serverOnly = false,
            string containerClass = null,
            Action<Exception, string, string> exceptionHandler = null,
            bool hydrateInPlace = false
        )
        {
            var scopedContext = htmlHelper.ViewContext.HttpContext.RequestServices.GetRequiredService<IReactScopedContext>();
            var reactComponent = scopedContext.CreateComponent<ReactComponent>(componentName: componentName);

            reactComponent.Props = props;
            reactComponent.ContainerId = containerId;
            reactComponent.ClientOnly = clientOnly;
            reactComponent.ServerOnly = serverOnly;
            reactComponent.ContainerClass = containerClass;
            
            if (exceptionHandler != null)
            {
                reactComponent.ExceptionHandler = exceptionHandler;
            }

            if (!string.IsNullOrEmpty(htmlTag))
            {
                reactComponent.ContainerTag = htmlTag;
            }

            await reactComponent.RenderHtml();

            return new ActionHtmlString(writer => reactComponent.WriteOutputHtmlTo(writer));
        }

        /// <summary>
        /// Renders the JavaScript required to initialise all components client-side. This will
        /// attach event handlers to the server-rendered HTML.
        /// </summary>
        /// <returns>JavaScript for all components</returns>
        public static IHtmlContent ReactInitJavaScript(this IHtmlHelper htmlHelper, bool clientOnly = false, bool delayedLambda = false)
        {
            var scopedContext = htmlHelper.ViewContext.HttpContext.RequestServices.GetRequiredService<IReactScopedContext>();
            var config = htmlHelper.ViewContext.HttpContext.RequestServices.GetRequiredService<ReactConfiguration>();

            return new ActionHtmlString(writer => WriteScriptTag(writer, bodyWriter => scopedContext.GetInitJavaScript(bodyWriter), config.ScriptNonceProvider, delayedLambda));
        }

        private static void WriteScriptTag(TextWriter writer, Action<TextWriter> bodyWriter, Func<string> ScriptNonceProvider, bool delayedLambda = false)
        {
            writer.Write("<script");
            if (ScriptNonceProvider != null)
            {
                writer.Write(" nonce=\"");
                writer.Write(ScriptNonceProvider());
                writer.Write("\"");
            }

            writer.Write(">");

            if (delayedLambda)
            {
                writer.Write("window.ReactJsAsyncInit = function() {");
            }

            bodyWriter(writer);

            if (delayedLambda)
            {
                writer.Write("};");
            }

            writer.Write("</script>");
        }
    }
}