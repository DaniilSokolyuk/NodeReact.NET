using System;
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

                options.EnvironmentVariables.Add("NODEREACT_MINWORKERS", config.StartEngines.ToString());
                options.EnvironmentVariables.Add("NODEREACT_MAXWORKERS", config.MaxEngines.ToString());
                options.EnvironmentVariables.Add("NODEREACT_MAXUSAGESPERFWORKER", config.MaxUsagesPerEngine.ToString());
            });
            services.Configure<OutOfProcessNodeJSServiceOptions>(options =>
            {
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
