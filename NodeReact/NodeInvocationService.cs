using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jering.Javascript.NodeJS;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace NodeReact
{
    public interface INodeInvocationService
    {
        Task<T> Invoke<T>(string function, object[] args, CancellationToken cancellationToken = default);
    }

    public class NodeInvocationService : INodeInvocationService
    {
        internal static readonly string MODULE_CACHE_IDENTIFIER = typeof(NodeInvocationService).Namespace;
        internal static readonly string BUNDLENAME = "bundle.js";

        private readonly IEmbeddedResourcesService _embeddedResourcesService;
        private readonly INodeJSService _nodeJsService;


        public NodeInvocationService(
            INodeJSService nodeJsService, 
            ReactConfiguration configuration, 
            IEmbeddedResourcesService embeddedResourcesService, 
            IOptions<NodeJSProcessOptions> nodeJsProcessOptions, 
            IServiceScopeFactory serviceScopeFactory)
        {
            _nodeJsService = nodeJsService;
            _embeddedResourcesService = embeddedResourcesService;

            using (IServiceScope scope = serviceScopeFactory.CreateScope())
            {
                IServiceProvider serviceProvider = scope.ServiceProvider;

                IWebHostEnvironment hostingEnvironment = serviceProvider.GetService<IWebHostEnvironment>();

                var requireFiles = configuration.ScriptFilesWithoutTransform
                    .Select(relativePath =>
                    {
                        if (relativePath.StartsWith(nodeJsProcessOptions.Value.ProjectPath))
                        {
                            return relativePath;
                        }

                        relativePath = relativePath.TrimStart('~').TrimStart('/');

                        return Path.GetFullPath(Path.Combine(hostingEnvironment?.WebRootPath ?? nodeJsProcessOptions.Value.ProjectPath, relativePath));
                    });

                //TODO: do this in configure
                nodeJsProcessOptions.Value.EnvironmentVariables.Add("NODEREACT_REQUIREFILES", string.Join(',', requireFiles));
            }
        }

        public Task<T> Invoke<T>(string function, object[] args, CancellationToken cancellationToken = default)
        {
            return _nodeJsService.InvokeFromStreamAsync<T>(
                () => _embeddedResourcesService.ReadAsStream(GetType().Assembly, BUNDLENAME),
                MODULE_CACHE_IDENTIFIER, 
                function, 
                args, 
                cancellationToken);
        }
    }
}
