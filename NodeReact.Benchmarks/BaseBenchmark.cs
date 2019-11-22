using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BenchmarkDotNet.Attributes;
using JavaScriptEngineSwitcher.ChakraCore;
using JavaScriptEngineSwitcher.Core;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using React;
using ZeroReact;

namespace NodeReact.Benchmarks
{
	[MemoryDiagnoser]
	[InProcess]
    public abstract class BaseBenchmark
	{
		[GlobalSetup]
		public void Setup()
		{
			PopulateTestData();
			RegisterZeroReactAndNodeReact();
            RegisterReact();
		}
		
		protected JObject _testData = JObject.FromObject(new Dictionary<string, string>() { ["name"] = "Tester" });

		protected IServiceProvider sp;

		protected void PopulateTestData()
		{
			for (int i = 0; i < 10; i++)
			{
				_testData.Add("key" + i, "value" + i);
			}
		}

        protected void RegisterZeroReactAndNodeReact()
		{
			var services = new ServiceCollection();

			services.AddZeroReactCore(
				config =>
				{
					config.AddScriptWithoutTransform("Sample.js");
					config.StartEngines = Environment.ProcessorCount;
					config.MaxEngines = Environment.ProcessorCount;
                    config.MaxUsagesPerEngine = 300;
					config.AllowJavaScriptPrecompilation = false;
				});

            services.AddNodeReact(
                config =>
                {
                    config.StartEngines = Environment.ProcessorCount;
                    config.MaxEngines = Environment.ProcessorCount;
                    
                    config.AddScriptWithoutTransform("SampleNode.js")
                        .AddScriptWithoutTransform("react.generated.min.js");
                });

            sp = services.BuildServiceProvider();
		}

		protected void RegisterReact()
		{
			Initializer.Initialize(registration => registration.AsSingleton());
			AssemblyRegistration.Container.Register<global::React.ICache, global::React.NullCache>();
			AssemblyRegistration.Container.Register<global::React.IFileSystem, global::NodeReact.Benchmarks.React.PhysicalFileSystem>();
			AssemblyRegistration.Container.Register<IReactEnvironment, ReactEnvironment>().AsMultiInstance();

			JsEngineSwitcher.Current.EngineFactories.Add(new ChakraCoreJsEngineFactory());
			JsEngineSwitcher.Current.DefaultEngineName = ChakraCoreJsEngine.EngineName;

			var configuration = ReactSiteConfiguration.Configuration;
			configuration
				.SetReuseJavaScriptEngines(true)
				.SetAllowJavaScriptPrecompilation(false);
			configuration
				.SetStartEngines(Environment.ProcessorCount)
				.SetMaxEngines(Environment.ProcessorCount)
				.SetMaxUsagesPerEngine(1000)
				.SetLoadBabel(false)
				.AddScriptWithoutTransform("Sample.js");
		}
		
		public class NoTextWriter : TextWriter
		{
			public override Encoding Encoding => Encoding.Unicode;
		}
	}
}
