using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Jering.Javascript.NodeJS;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NodeReact.Components;
using NodeReact.Jering.Javascript.NodeJS;

namespace NodeReact
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNodeReact(this IServiceCollection services, Action<ReactConfiguration> configuration = null)
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
                config.ConfigureNodeJSProcessOptions?.Invoke(options);
            });
            services.Configure<OutOfProcessNodeJSServiceOptions>(options =>
            {
                options.Concurrency = Concurrency.MultiProcess;
                options.ConcurrencyDegree = config.EnginesCount;
                
                if (config.UseDebugReact)
                {
                    options.EnableFileWatching = true;
                    var fw = config.ScriptFilesWithoutTransform.Select(Path.GetFileName).Distinct().ToArray();
                    options.WatchFileNamePatterns = fw;
                    options.WatchSubdirectories = true;
                }

                config.ConfigureOutOfProcessNodeJSServiceOptions?.Invoke(options);
            });

            services.Replace(new ServiceDescriptor(
                typeof(IJsonService),
                typeof(CustomJsonService), 
                ServiceLifetime.Singleton));

            services.AddScoped<IReactScopedContext, ReactScopedContext>();

            services.AddTransient<ReactComponent>();
            services.AddTransient<ReactRouterComponent>();

            return services;
        }
    }
}
