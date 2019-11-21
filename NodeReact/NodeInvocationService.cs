using System.Buffers;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jering.Javascript.NodeJS;
using Microsoft.AspNetCore.Hosting;
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
        private readonly ReactConfiguration _configuration;
        private readonly string[] _mappedFilesPaths;
        private readonly string _bundleName;
        

        public NodeInvocationService(
            INodeJSService nodeJsService, 
            ReactConfiguration configuration, 
            IEmbeddedResourcesService embeddedResourcesService, 
            IHostingEnvironment hostingEnvironment,
            IOptions<NodeJSProcessOptions> nodeJsProcessOptions)
        {
            _nodeJsService = nodeJsService;
            _configuration = configuration;
            _embeddedResourcesService = embeddedResourcesService;

            _bundleName = configuration.UseDebugReact ? "WithoutReact" : "WithoutReactMin";

            _mappedFilesPaths = configuration.ScriptFilesWithoutTransform
                .Select(relativePath =>
                {
                    if (relativePath.StartsWith(hostingEnvironment.WebRootPath))
                    {
                        return relativePath;
                    }

                    relativePath = relativePath.TrimStart('~').TrimStart('/');

                    return Path.GetFullPath(Path.Combine(hostingEnvironment.WebRootPath, relativePath));
                })
                .ToArray();
        }

        public async Task<T> Invoke<T>(string function, IMemoryOwner<char> code, CancellationToken cancellationToken = default)
        {
            var str = new string(code.Memory.Span);

            var args = new object[] { _mappedFilesPaths, str };

            // Invoke from cache
            (bool success, T result) = await _nodeJsService.TryInvokeFromCacheAsync<T>(MODULE_CACHE_IDENTIFIER, function, args, cancellationToken).ConfigureAwait(false);
            if (success)
            {
                return result;
            }

            using (Stream moduleStream = _embeddedResourcesService.ReadAsStream(GetType().Assembly, _bundleName))
            {
                return await _nodeJsService.InvokeFromStreamAsync<T>(moduleStream, MODULE_CACHE_IDENTIFIER, function, args, cancellationToken).ConfigureAwait(false);
            }

        }
    }
}
