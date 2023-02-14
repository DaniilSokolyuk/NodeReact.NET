using System;
using System.IO;
using System.Text;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace NodeReact.Benchmarks
{
	[MemoryDiagnoser]
	//[InProcess]
    public abstract class BaseBenchmark
	{
		[GlobalSetup]
		public void Setup()
		{
			RegisterNodeReact();
		}

        protected JObject _testData = JObject.Parse(File.ReadAllText("hugeComponentData.json"));

		protected IServiceProvider sp;

        protected void RegisterNodeReact()
		{
			var services = new ServiceCollection();

            services.AddNodeReact(
                config =>
                {
	                config.ConfigureNewtonsoftJsonPropsSerializer(_ => {});
	                
                    config.EnginesCount = Environment.ProcessorCount;

                    config.AddScriptWithoutTransform("hugeBundle.js");
                });

            sp = services.BuildServiceProvider();
		}

        public class NoTextWriter : TextWriter
		{
			public override Encoding Encoding => Encoding.Unicode;
		}
	}
}
