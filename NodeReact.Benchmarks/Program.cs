using System;
using System.Threading.Tasks;
using BenchmarkDotNet.Running;

namespace NodeReact.Benchmarks
{
	public static class Program
	{
		public static async Task Main(string[] args)
		{
            //var tt = new SingleComponentBenchmark();
            //tt.Setup();

            //for (int i = 0; i < 10000000; i++)
            //{
            //    await tt.NodeReact_RenderRouterSingle();
            //}


            BenchmarkRunner.Run<SingleComponentBenchmark>();
           // BenchmarkRunner.Run<WebSimulateBenchmark>();
            Console.ReadKey();
		}
	}
}
