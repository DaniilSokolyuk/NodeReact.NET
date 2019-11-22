using System;
using System.Buffers;
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
        Task<T> Invoke<T>(string function, IMemoryOwner<char> code, CancellationToken cancellationToken = default);
    }

    public class NodeInvocationService : INodeInvocationService
    {
        internal static readonly string MODULE_CACHE_IDENTIFIER = typeof(NodeInvocationService).Namespace;

        private readonly IEmbeddedResourcesService _embeddedResourcesService;
        private readonly INodeJSService _nodeJsService;
        private readonly string _bundleName;


        public NodeInvocationService(
            INodeJSService nodeJsService, 
            ReactConfiguration configuration, 
            IEmbeddedResourcesService embeddedResourcesService, 
            IOptions<NodeJSProcessOptions> nodeJsProcessOptions, 
            IServiceScopeFactory serviceScopeFactory)
        {
            _nodeJsService = nodeJsService;
            _embeddedResourcesService = embeddedResourcesService;
            _bundleName = configuration.UseDebugReact ? "interopBundle" : "interopBundleMin";

            using (IServiceScope scope = serviceScopeFactory.CreateScope())
            {
                IServiceProvider serviceProvider = scope.ServiceProvider;

                IHostingEnvironment hostingEnvironment = serviceProvider.GetService<IHostingEnvironment>();

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

        public async Task<T> Invoke<T>(string function, IMemoryOwner<char> code, CancellationToken cancellationToken = default)
        {
            var str = new string(code.Memory.Span);

            var args = new object[] { str };

            // Invoke from cache
            (bool success, T result) = await _nodeJsService.TryInvokeFromCacheAsync<T>(MODULE_CACHE_IDENTIFIER, function, args, cancellationToken);
            if (success)
            {
                return result;
            }

            using (Stream moduleStream = _embeddedResourcesService.ReadAsStream(GetType().Assembly, _bundleName))
            {
                return await _nodeJsService.InvokeFromStreamAsync<T>(moduleStream, MODULE_CACHE_IDENTIFIER, function, args, cancellationToken);
            }

        }
    }
}
