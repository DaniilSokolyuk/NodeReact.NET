using System;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using React;

namespace NodeReact.Benchmarks
{
	public class WebSimulateBenchmark : BaseBenchmark
	{
		private readonly NoTextWriter tk = new NoTextWriter(); //TODO: simulate PagedBufferedTextWriter

        [Benchmark]
        public async Task NodeReact_WebSimulation()
        {
            var tasks = Enumerable.Range(0, 20).Select(async x =>
            {
                using (var scope = sp.CreateScope())
                {
                    foreach (var ind in Enumerable.Range(0, 30))
                    {
                        var reactContext = scope.ServiceProvider.GetRequiredService<NodeReact.IReactScopedContext>();

                        var component = reactContext.CreateComponent<NodeReact.Components.ReactComponent>("HelloWorld");
                        component.Props = _testData;

                        await component.RenderHtml();

                        component.WriteOutputHtmlTo(tk);
                    }

                    scope.ServiceProvider.GetRequiredService<NodeReact.IReactScopedContext>().GetInitJavaScript(tk);
                }
            });

            await Task.WhenAll(tasks);
        }


        [Benchmark]
        public async Task ZeroReact_WebSimulation()
        {
            var tasks = Enumerable.Range(0, 20).Select(async x =>
            {
                using (var scope = sp.CreateScope())
                {
                    foreach (var ind in Enumerable.Range(0, 30))
                    {
                        var reactContext = scope.ServiceProvider.GetRequiredService<ZeroReact.IReactScopedContext>();

                        var component = reactContext.CreateComponent<ZeroReact.Components.ReactComponent>("HelloWorld");
                        component.Props = _testData;

                        await component.RenderHtml();

                        component.WriteOutputHtmlTo(tk);
                    }

                    scope.ServiceProvider.GetRequiredService<ZeroReact.IReactScopedContext>().GetInitJavaScript(tk);
                }
            });

            await Task.WhenAll(tasks);
        }

        [Benchmark]
        public void ReactJSNet_WebSimulation()
        {
            Parallel.For(0, 20, i =>
            {
                var environment = AssemblyRegistration.Container.Resolve<IReactEnvironment>();
                foreach (var ind in Enumerable.Range(0, 30))
                {
                    var component = environment.CreateComponent("HelloWorld", _testData);

                    component.RenderHtml(tk);
                    environment.ReturnEngineToPool();
                }

                environment.GetInitJavaScript(tk);
                ((IDisposable) environment).Dispose();
            });
        }
    }
}
