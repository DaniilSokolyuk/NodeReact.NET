# NodeReact.NET [![NuGet Version](https://img.shields.io/nuget/v/NodeReact.svg)](https://www.nuget.org/packages/NodeReact/) 
Library to render React library components on the server-side with C# as well as on the client.

# Migration from ReactJS.NET
ReactJS.NET api is almost completely compatible except
* Not supported On-the-fly JSX to JavaScript compilation (only AddScriptWithoutTransform)
* Not supported render functions (ReactJS.NET v4 feature)

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

BenchmarkDotNet=v0.12.0, OS=Windows 10.0.17763.864 (1809/October2018Update/Redstone5)
Intel Core i7-8750H CPU 2.20GHz (Coffee Lake), 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=3.1.100
  [Host]     : .NET Core 3.1.0 (CoreCLR 4.700.19.56402, CoreFX 4.700.19.56404), X64 RyuJIT
  DefaultJob : .NET Core 3.1.0 (CoreCLR 4.700.19.56402, CoreFX 4.700.19.56404), X64 RyuJIT


```
|                       Method |     Mean |    Error |   StdDev |   Median | Gen 0 | Gen 1 | Gen 2 |  Allocated |
|----------------------------- |---------:|---------:|---------:|---------:|------:|------:|------:|-----------:|
| NodeReact_RenderRouterSingle | 22.10 ms | 0.498 ms | 1.460 ms | 22.14 ms |     - |     - |     - |   10.15 KB |
|       NodeReact_RenderSingle | 21.79 ms | 0.508 ms | 1.490 ms | 21.76 ms |     - |     - |     - |   10.23 KB |
| ZeroReact_RenderRouterSingle | 47.90 ms | 2.089 ms | 5.787 ms | 46.18 ms |     - |     - |     - |     7.4 KB |
|       ZeroReact_RenderSingle | 42.75 ms | 2.475 ms | 6.900 ms | 42.10 ms |     - |     - |     - |    4.73 KB |
|         ReactJs_RenderSingle | 50.19 ms | 1.149 ms | 3.277 ms | 50.89 ms |     - |     - |     - | 1305.15 KB |



Web Simulation (20 parallel req., 2 components per request)
``` ini

BenchmarkDotNet=v0.12.0, OS=Windows 10.0.17763.864 (1809/October2018Update/Redstone5)
Intel Core i7-8750H CPU 2.20GHz (Coffee Lake), 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=3.1.100
  [Host]     : .NET Core 3.1.0 (CoreCLR 4.700.19.56402, CoreFX 4.700.19.56404), X64 RyuJIT
  DefaultJob : .NET Core 3.1.0 (CoreCLR 4.700.19.56402, CoreFX 4.700.19.56404), X64 RyuJIT


```
|                   Method |     Mean |    Error |   StdDev |     Gen 0 |     Gen 1 |     Gen 2 |   Allocated |
|------------------------- |---------:|---------:|---------:|----------:|----------:|----------:|------------:|
|  NodeReact_WebSimulation | 203.9 ms |  4.10 ms |  7.70 ms |         - |         - |         - |   416.34 KB |
|  ZeroReact_WebSimulation | 423.1 ms | 11.05 ms | 31.90 ms |         - |         - |         - |   376.23 KB |
| ReactJSNet_WebSimulation | 589.7 ms | 25.05 ms | 73.86 ms | 5000.0000 | 3000.0000 | 2000.0000 | 63152.93 KB |

