# NodeReact.NET [![NuGet Version](https://img.shields.io/nuget/v/NodeReact.svg)](https://www.nuget.org/packages/NodeReact/) 
Library to render React library components on the server-side with C# as well as on the client.

# Features
* Streaming, waiting for data, Suspnse and hydrateRoot... support via custom [View Engine](https://github.com/DaniilSokolyuk/NodeReact.NET/tree/master/NodeReact.Sample.Webpack.AspNetCor.Streaming)
* High performance
* Truly async

# Migration from ReactJS.NET
ReactJS.NET api is almost completely compatible except
* Not supported On-the-fly JSX to JavaScript compilation (only AddScriptWithoutTransform)
* Not supported render functions (ReactJS.NET v4 feature)

1. Make sure you use @await Html.PartialAsync and @await Html.RenderAsync on cshtml views, synchronous calls can deadlock application 
2. Replace 
* @Html.React to @await Html.ReactAsync
* @Html.ReactWithInit to @await ReactAsync 
* @Html.ReactRouter to @await Html.ReactRouterAsync
3. Register NodeReact in service collection, example [here](https://github.com/DaniilSokolyuk/NodeReact.NET/blob/master/NodeReact.Sample.Webpack.AspNetCore/Startup.cs)

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
