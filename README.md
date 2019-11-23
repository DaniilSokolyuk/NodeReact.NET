# NodeReact.NET [![NuGet Version](https://img.shields.io/nuget/v/NodeReact.svg)](https://www.nuget.org/packages/NodeReact/) 

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

# Benchmarks
``` ini

BenchmarkDotNet=v0.12.0, OS=Windows 10.0.17763.475 (1809/October2018Update/Redstone5)
Intel Core i7-7700 CPU 3.60GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=3.0.100
  [Host] : .NET Core 3.0.0 (CoreCLR 4.700.19.46205, CoreFX 4.700.19.46214), X64 RyuJIT

Job=InProcess  Toolchain=InProcessEmitToolchain  

```
|                 Method |      Mean |     Error |    StdDev |    Median | Gen 0 | Gen 1 | Gen 2 | Allocated |
|----------------------- |----------:|----------:|----------:|----------:|------:|------:|------:|----------:|
| NodeReact_RenderSingle |  8.380 ms | 0.4387 ms | 1.2867 ms |  7.831 ms |     - |     - |     - | 638.34 KB |
| ZeroReact_RenderSingle | 14.106 ms | 1.3290 ms | 3.8345 ms | 13.604 ms |     - |     - |     - |   4.73 KB |
|   ReactJs_RenderSingle | 12.027 ms | 0.3590 ms | 0.9705 ms | 12.018 ms |     - |     - |     - | 898.66 KB |
