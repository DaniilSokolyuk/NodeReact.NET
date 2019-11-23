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

Single Component
``` ini

BenchmarkDotNet=v0.12.0, OS=Windows 10.0.17763.475 (1809/October2018Update/Redstone5)
Intel Core i7-7700 CPU 3.60GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=3.0.100
  [Host] : .NET Core 3.0.0 (CoreCLR 4.700.19.46205, CoreFX 4.700.19.46214), X64 RyuJIT

Job=InProcess  Toolchain=InProcessEmitToolchain  

```
|                 Method |     Mean |    Error |   StdDev |   Median | Gen 0 | Gen 1 | Gen 2 |  Allocated |
|----------------------- |---------:|---------:|---------:|---------:|------:|------:|------:|-----------:|
| NodeReact_RenderSingle | 27.32 ms | 1.047 ms | 3.038 ms | 26.93 ms |     - |     - |     - | 1262.37 KB |
| ZeroReact_RenderSingle | 48.73 ms | 2.198 ms | 6.165 ms | 48.48 ms |     - |     - |     - |    4.73 KB |
|   ReactJs_RenderSingle | 55.89 ms | 2.289 ms | 6.714 ms | 54.03 ms |     - |     - |     - | 1305.15 KB |


Web Simulation (20 parallel req., 2 components per request)
``` ini

BenchmarkDotNet=v0.12.0, OS=Windows 10.0.17763.475 (1809/October2018Update/Redstone5)
Intel Core i7-7700 CPU 3.60GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=3.0.100
  [Host] : .NET Core 3.0.0 (CoreCLR 4.700.19.46205, CoreFX 4.700.19.46214), X64 RyuJIT

Job=InProcess  Toolchain=InProcessEmitToolchain  

```
|                   Method |     Mean |    Error |    StdDev |     Gen 0 |     Gen 1 |     Gen 2 | Allocated |
|------------------------- |---------:|---------:|----------:|----------:|----------:|----------:|----------:|
|  NodeReact_WebSimulation | 276.1 ms |  8.06 ms |  23.77 ms | 5000.0000 | 1000.0000 |         - |  49.36 MB |
|  ZeroReact_WebSimulation | 492.4 ms | 24.94 ms |  72.76 ms |         - |         - |         - |    1.6 MB |
| ReactJSNet_WebSimulation | 814.0 ms | 37.94 ms | 111.26 ms | 5000.0000 | 3000.0000 | 2000.0000 |  66.98 MB |
