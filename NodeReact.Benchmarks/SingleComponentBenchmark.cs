using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using React;
using ZeroReactComponent = ZeroReact.Components.ReactComponent;
using NodeReactComponent = NodeReact.Components.ReactComponent;

namespace NodeReact.Benchmarks
{
	public class SingleComponentBenchmark : BaseBenchmark
	{
		private readonly NoTextWriter tk = new NoTextWriter();

        [Benchmark]
        public async Task NodeReact_RenderSingle()
        {
            using (var scope = sp.CreateScope())
            {
                var reactContext = scope.ServiceProvider.GetRequiredService<NodeReact.IReactScopedContext>();

                var component = reactContext.CreateComponent<NodeReactComponent>("HelloWorld");
                component.Props = _testData;
                component.ServerOnly = true;

                await component.RenderHtml();

                component.WriteOutputHtmlTo(tk);
            }
        }

        [Benchmark]
        public async Task ZeroReact_RenderSingle()
        {
            using (var scope = sp.CreateScope())
            {
                var reactContext = scope.ServiceProvider.GetRequiredService<ZeroReact.IReactScopedContext>();

                var component = reactContext.CreateComponent<ZeroReactComponent>("HelloWorld");
                component.Props = _testData;
                component.ServerOnly = true;

                await component.RenderHtml();

                component.WriteOutputHtmlTo(tk);
            }
        }

        [Benchmark]
	    public void ReactJs_RenderSingle()
	    {
		    var environment = AssemblyRegistration.Container.Resolve<IReactEnvironment>();
		    var component = environment.CreateComponent("HelloWorld", _testData, serverOnly: true);

		    component.RenderHtml(tk, renderServerOnly: true);
		    environment.ReturnEngineToPool();
        }
    }
}
