using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace NodeReact.Benchmarks
{
    public class SingleComponentBenchmark : BaseBenchmark
    {
        private readonly NoTextWriter tk = new NoTextWriter();

        [Benchmark]
        public async Task NodeReact_RenderRouterSingle()
        {
            using (var scope = sp.CreateScope())
            {
                var reactContext = scope.ServiceProvider.GetRequiredService<NodeReact.IReactScopedContext>();

                var component = reactContext.CreateComponent<NodeReact.Components.ReactRouterComponent>("__desktopComponents.App");
                component.Props = _testData;
                component.ServerOnly = true;
                component.Location = "/movie/246436/";

                await component.RenderRouterWithContext();

                component.WriteOutputHtmlTo(tk);
            }
        }

        [Benchmark]
        public async Task NodeReact_RenderSingle()
        {
            using (var scope = sp.CreateScope())
            {
                var reactContext = scope.ServiceProvider.GetRequiredService<NodeReact.IReactScopedContext>();

                var component = reactContext.CreateComponent<NodeReact.Components.ReactComponent>("__components.MovieAboutPage");
                component.Props = _testData;
                component.ServerOnly = true;

                await component.RenderHtml();

                component.WriteOutputHtmlTo(tk);
            }
        }
    }
}
