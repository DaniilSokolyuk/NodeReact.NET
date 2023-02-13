using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace NodeReact.AspNetCore.ViewEngine;

/// <summary>
/// NodeReact view engine.
/// </summary>
public class NodeReactViewEngine : IViewEngine
{
    private readonly string _prefix;
    private readonly string _defaultView;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="prefix">Search prefix, if the name of the View starts with a prefix it will try to render a component whose name comes after the pref </param>
    /// <param name="defaultView">If the prefix is empty or the component name is missing, it will try to render defaultView component</param>
    public NodeReactViewEngine(string prefix = "$", string defaultView = "")
    {
        _prefix = prefix;
        _defaultView = defaultView;
    }
    
    public ViewEngineResult FindView(ActionContext context, string viewName, bool isMainPage)
    {
        return ViewEngineResult.NotFound(viewName, Enumerable.Empty<string>());
    }

    public ViewEngineResult GetView(string executingFilePath, string viewPath, bool isMainPage)
    {
        // if the view name starts with the prefix, try to render a component whose name comes after the prefix
        // if the prefix is empty or the component name is missing, try to render defaultView component
        if (_prefix.Length > 0)
        {
            var componentName = viewPath.StartsWith(_prefix) ? viewPath.Substring(_prefix.Length) : _defaultView;
            if (componentName.Length > 0)
            {
                return ViewEngineResult.Found(viewPath, new NodeReactView(componentName));
            }
        } 
        else if (_defaultView.Length > 0)
        {
            return ViewEngineResult.Found(_defaultView, new NodeReactView(_defaultView));
        }

        return ViewEngineResult.NotFound(viewPath, Enumerable.Empty<string>());
    }
}