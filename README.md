# NodeReact.NET 

Aalternative to [ReactJS.NET](https://github.com/reactjs/React.NET) with rendering in separated node.js process

* Not supported On-the-fly JSX to JavaScript compilation (only AddScriptWithoutTransform)
* Not supported render functions (ReactJS.NET v4 feature)

# Migration from ReactJS.NET
1. Make sure you use @await Html.PartialAsync and @await Html.RenderAsync on cshtml views, synchronous calls can deadlock application 
2. Replace 
* @Html.React to @await Html.ReactAsync
* @Html.ReactWithInit to @await ReactWithInitAsync
* @Html.ReactRouter to @await Html.ReactRouterAsync
3. Register NodeReact in service collection, example [here](https://github.com/DaniilSokolyuk/NodeReact.NET/blob/14d48c55abc4baabe6562d2c0cc79a4186286d54/NodeReact.Sample.Webpack.AspNetCore/Startup.cs#L22)

Ensure that any your server bundle define global variables for react like
```
global.React = require('react');
global.ReactDOM = require('react-dom');
global.ReactDOMServer = require('react-dom/server');
```

Make sure you have Node.JS installed

Configure [Javascript.NodeJS](https://github.com/JeringTech/Javascript.NodeJS) for debug example
```
services.AddNodeReact(
    config =>
    {
        config.ConfigureNodeJSProcessOptions = opt => opt.NodeAndV8Options = "--inspect-brk";
        config.ConfigureOutOfProcessNodeJSServiceOptions = opt => opt.TimeoutMS = -1;
        config.AddScriptWithoutTransform("~/server.bundle.js");
        config.UseDebugReact = true;
    });
```
