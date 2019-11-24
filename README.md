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

# Benchmarks (version 1.0.1 !)

Single Component
``` ini

BenchmarkDotNet=v0.12.0, OS=Windows 10.0.17763.475 (1809/October2018Update/Redstone5)
Intel Core i7-7700 CPU 3.60GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=3.0.100
  [Host] : .NET Core 3.0.0 (CoreCLR 4.700.19.46205, CoreFX 4.700.19.46214), X64 RyuJIT

Job=InProcess  Toolchain=InProcessEmitToolchain  

```
|                       Method |     Mean |    Error |   StdDev |   Median | Gen 0 | Gen 1 | Gen 2 |  Allocated |
|----------------------------- |---------:|---------:|---------:|---------:|------:|------:|------:|-----------:|
| NodeReact_RenderRouterSingle | 25.52 ms | 0.742 ms | 2.186 ms | 25.48 ms |     - |     - |     - |   10.09 KB |
|       NodeReact_RenderSingle | 24.52 ms | 0.596 ms | 1.720 ms | 24.41 ms |     - |     - |     - |   10.41 KB |
|       ZeroReact_RenderSingle | 48.26 ms | 2.493 ms | 7.031 ms | 47.95 ms |     - |     - |     - |    4.73 KB |
| ZeroReact_RenderRouterSingle | 50.14 ms | 1.335 ms | 3.632 ms | 48.97 ms |     - |     - |     - |     7.4 KB |
|         ReactJs_RenderSingle | 54.96 ms | 1.856 ms | 5.174 ms | 54.33 ms |     - |     - |     - | 1305.15 KB |


Web Simulation (20 parallel req., 2 components per request)
``` ini

BenchmarkDotNet=v0.12.0, OS=Windows 10.0.17763.475 (1809/October2018Update/Redstone5)
Intel Core i7-7700 CPU 3.60GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=3.0.100
  [Host] : .NET Core 3.0.0 (CoreCLR 4.700.19.46205, CoreFX 4.700.19.46214), X64 RyuJIT

Job=InProcess  Toolchain=InProcessEmitToolchain  

```
|                   Method |     Mean |    Error |    StdDev |     Gen 0 |     Gen 1 |     Gen 2 |   Allocated |
|------------------------- |---------:|---------:|----------:|----------:|----------:|----------:|------------:|
|  NodeReact_WebSimulation | 252.7 ms |  5.61 ms |  13.10 ms |         - |         - |         - |   418.76 KB |
|  ZeroReact_WebSimulation | 445.5 ms | 10.18 ms |  29.52 ms |         - |         - |         - |  1257.36 KB |
| ReactJSNet_WebSimulation | 909.4 ms | 52.93 ms | 154.40 ms | 8000.0000 | 6000.0000 | 5000.0000 | 71924.75 KB |

