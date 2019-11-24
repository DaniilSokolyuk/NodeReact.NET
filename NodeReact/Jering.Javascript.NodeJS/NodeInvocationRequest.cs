using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json.Serialization;
using Jering.Javascript.NodeJS;

namespace NodeReact
{
    public class NodeInvocationRequest
    {
        public NodeInvocationRequest(InvocationRequest request)
        {
            ModuleSource = request.ModuleSource;
            ModuleSourceType = request.ModuleSourceType;
            NewCacheIdentifier = request.NewCacheIdentifier;
            ExportName = request.ExportName;
            Args = request.Args;
            ModuleStreamSource = request.ModuleStreamSource;
        }

        /// <summary>
        /// Gets the source type of the module to be invoked.
        /// </summary>
        public ModuleSourceType ModuleSourceType { get; }

        /// <summary>
        /// Gets the source of the module to be invoked.
        /// </summary>
        public string ModuleSource { get; }

        /// <summary>
        /// Gets the new cache identifier for the module to be invoked.
        /// </summary>
        public string NewCacheIdentifier { get; }

        /// <summary>
        /// Gets the name of the function in the module's exports to invoke.
        /// </summary>
        public string ExportName { get; }

        /// <summary>
        /// Gets the arguments for the function to invoke.
        /// </summary>
        public object[] Args { get; }

        [JsonIgnore]
        public Stream ModuleStreamSource { get; }
    }
}
