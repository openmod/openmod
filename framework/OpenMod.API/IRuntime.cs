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
        /// <summary>
        /// Checks if the runtime is shutting down or disposing.
        /// </summary>
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

        /// <summary>
        /// Gets the OpenMod runtime version.
        /// </summary>
        SemVersion Version { get; }

        /// <summary>
        /// Gets the commandline arguments.
        /// </summary>
        string[] CommandlineArgs { get; }

        /// <summary>
        /// Gets the runtime status. 
        /// </summary>
        RuntimeStatus Status { get; }

        /// <summary>
        /// Rebuilds the DI container and reloads all plugins, services, configurations etc.
        /// </summary>
        Task PerformSoftReloadAsync();

        /// <summary>
        /// Gets the .NET generic host instance. Returns null if the host is not loaded yet.
        /// </summary>
        IHost? Host { get; }

        /// <summary>
        /// Information about the OpenMod host. Returns null if the host is not loaded yet.
        /// </summary>
        IHostInformation? HostInformation { get; }

        /// <summary>
        /// Gets the OpenMod host assemblies.
        /// </summary>
        IReadOnlyCollection<Assembly> HostAssemblies { get; }
    }
}