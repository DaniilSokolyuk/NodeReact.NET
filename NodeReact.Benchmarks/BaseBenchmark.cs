using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BenchmarkDotNet.Attributes;
using JavaScriptEngineSwitcher.Core;
using JavaScriptEngineSwitcher.V8;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using React;

namespace NodeReact.Benchmarks
{
	[MemoryDiagnoser]
	//[InProcess]
    public abstract class BaseBenchmark
	{
		[GlobalSetup]
		public void Setup()
		{
			RegisterZeroReactAndNodeReact();
            RegisterReact();
		}

        protected JObject _testData = JObject.Parse(File.ReadAllText("hugeComponentData.json"));

		protected IServiceProvider sp;

        protected void RegisterZeroReactAndNodeReact()
		{
			var services = new ServiceCollection();

            services.AddNodeReact(
                config =>
                {
                    config.EnginesCount = Environment.ProcessorCount;

                    config.AddScriptWithoutTransform("hugeBundle.js");
                });

            sp = services.BuildServiceProvider();
		}

		protected void RegisterReact()
		{
			Initializer.Initialize(registration => registration.AsSingleton());
			AssemblyRegistration.Container.Register<global::React.ICache, global::React.NullCache>();
			AssemblyRegistration.Container.Register<global::React.IFileSystem, global::NodeReact.Benchmarks.React.PhysicalFileSystem>();
			AssemblyRegistration.Container.Register<IReactEnvironment, ReactEnvironment>().AsMultiInstance();

			JsEngineSwitcher.Current.EngineFactories.Add(new V8JsEngineFactory());
            JsEngineSwitcher.Current.DefaultEngineName = V8JsEngine.EngineName;

			var configuration = ReactSiteConfiguration.Configuration;
			configuration
				.SetReuseJavaScriptEngines(true)
				.SetAllowJavaScriptPrecompilation(false)
				.SetStartEngines(Environment.ProcessorCount)
				.SetMaxEngines(Environment.ProcessorCount)
				.SetMaxUsagesPerEngine(int.MaxValue)
				.SetLoadBabel(false)
                .SetLoadReact(false)
				.AddScriptWithoutTransform("hugeBundle.js");
		}
		
		public class NoTextWriter : TextWriter
		{
			public override Encoding Encoding => Encoding.Unicode;
		}
	}
}
