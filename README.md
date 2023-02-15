# NodeReact.NET [![NuGet Version](https://img.shields.io/nuget/v/NodeReact.svg)](https://www.nuget.org/packages/NodeReact/) 
Library to render React library components on the server-side with C# as well as on the client.

# Features
* Streaming, waiting for data, Suspnse and hydrateRoot... support via custom [View Engine](https://github.com/DaniilSokolyuk/NodeReact.NET/tree/master/NodeReact.Sample.Streaming)
* High performance
* Truly async

# Migration from ReactJS.NET
ReactJS.NET api is almost completely compatible except
* Not supported On-the-fly JSX to JavaScript compilation (only AddScriptWithoutTransform)
* Not supported render functions (ReactJS.NET v4 feature)

1. Make sure you use @await Html.PartialAsync and @await Html.RenderAsync on cshtml views, synchronous calls can deadlock application 
2. Replace 
* @Html.React to @await Html.ReactAsync
* @Html.ReactWithInit to @await Html.ReactAsync 
* @Html.ReactRouter to @await Html.ReactRouterAsync
3. Register NodeReact in service collection, example [here](https://github.com/DaniilSokolyuk/NodeReact.NET/blob/master/NodeReact.Sample/Startup.cs)

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

* Why isn't on-the-fly JSX to JavaScript compilation supported?
We do not support real-time JavaScript conversion. This is because there are many different build tools, compilers, transpilers, and programming languages, and configuring them all is not a simple task. It is impossible to create a one-size-fits-all compilation solution that is both high-performing and efficient. Instead, we suggest that you create your own server bundle by examining the [sample](https://github.com/DaniilSokolyuk/NodeReact.NET/tree/master/NodeReact.Sample) provided in our repository.
* Why isn't on-the-fly JSX to JavaScript compilation supported?
If you have any ideas, please feel free to create a pull request as I am currently unsure how to implement this feature without a significant impact on performance

