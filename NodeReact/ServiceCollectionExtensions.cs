using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Jering.Javascript.NodeJS;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NodeReact.Components;

namespace NodeReact
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNodeReact(this IServiceCollection services,
            Action<ReactConfiguration> configuration = null)
        {
            var config = new ReactConfiguration();
            configuration?.Invoke(config);

            services.AddSingleton(config);

            services.AddSingleton<IComponentNameInvalidator, ComponentNameInvalidator>();
            services.AddSingleton<IReactIdGenerator, ReactIdGenerator>();
            services.AddSingleton<INodeInvocationService, NodeInvocationService>();
            
            services.AddNodeJS();
            services.Configure<NodeJSProcessOptions>(options =>
            {
                options.EnvironmentVariables.Add("NODEREACT_FILEWATCHERDEBOUNCE", config.FileWatcherDebounceMs.ToString());

                config.ConfigureNodeJSProcessOptionsAction?.Invoke(options);
            });
            services.Configure<OutOfProcessNodeJSServiceOptions>(options =>
            {
                options.Concurrency = Concurrency.MultiProcess;
                options.ConcurrencyDegree = config.EnginesCount;

                config.ConfigureOutOfProcessNodeJSServiceOptionsAction?.Invoke(options);
            });
            services.Configure<HttpNodeJSServiceOptions>(options =>
            {
                config.ConfigureHttpNodeJSServiceOptionsAction?.Invoke(options);
            });


            services.Replace(new ServiceDescriptor(
                typeof(IJsonService),
                typeof(NodeReactJeringNodeJsonService),
                ServiceLifetime.Singleton));

            services.AddScoped<IReactScopedContext, ReactScopedContext>();

            services.AddTransient<ReactComponent>();
            services.AddTransient<ReactRouterComponent>();

            return services;
        }
    }
}