using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using OpenMod.API.Ioc;
using Semver;

namespace OpenMod.API
{
    /// <summary>
    /// The OpenMod runtime is responsible for initializing OpenMod.
    /// </summary>
    [Service]
    public interface IRuntime : IOpenModComponent
    {
        /// <value>
        /// <b>True</b> if the runtime is shutting down / disposing.
        /// </value>
        public bool IsDisposing { get; }

        /// <summary>
        /// Initializes the runtime.
        /// </summary>
        /// <returns>The .NET Generic Host interface.</returns>
        Task<IHost> InitAsync(List<Assembly> openModHostAssemblies, RuntimeInitParameters parameters,
            Func<IHostBuilder> hostBuilder);

        /// <summary>
        /// Shuts OpenMod down gracefully and disposes all services.
        /// </summary>
        Task ShutdownAsync();

        /// <value>
        /// The OpenMod runtime version.
        /// </value>
        SemVersion Version { get; }

        /// <value>
        /// The commandline arguments.
        /// </value>
        string[] CommandlineArgs { get; }

        /// <value>
        /// The runtime status. 
        /// </value>
        RuntimeStatus Status { get; }

        /// <summary>
        /// Rebuilds the DI container and reloads all plugins, services, configurations etc.
        /// </summary>
        Task PerformSoftReloadAsync();

        /// <value>
        /// The .NET generic host instance. Can be null if the host is not loaded yet.
        /// </value>
        IHost? Host { get; }

        /// <value>
        ///  Information about the OpenMod host. Can be null if the host is not loaded yet.
        /// </value>
        IHostInformation? HostInformation { get; }
    }
}