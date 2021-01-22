using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenMod.API.Ioc;
using Semver;

namespace OpenMod.API
{
    /// <summary>
    ///     Defines the OpenMod Runtime. This class is responsible for initializing OpenMod.
    /// </summary>
    [Service]
    public interface IRuntime : IOpenModComponent
    {
        /// <summary>
        ///   Checks if the runtime is shutting down / disposing.
        /// </summary>
        public bool IsDisposing { get; }

        /// <summary>
        ///     Initializes the runtime.
        /// </summary>
        /// <returns></returns>
        Task<IHost> InitAsync(List<Assembly> openModHostAssemblies, RuntimeInitParameters parameters, Func<IHostBuilder> hostBuilder);

        /// <summary>
        ///     Shuts down OpenMod and disposes all services.
        /// </summary>
        Task ShutdownAsync();

        /// <summary>
        ///     Gets the OpenMod runtime version.
        /// </summary>
        SemVersion Version { get; }

        /// <summary>
        ///    Commandline arguments.
        /// </summary>
        string[] CommandlineArgs { get; }

        /// <summary>
        ///    The runtime status.
        /// </summary>
        RuntimeStatus Status { get; }

        /// <summary>
        ///   Reloads all plugins, configurations, etc. and rebuilds the DI container
        /// </summary>
        /// <returns></returns>
        Task PerformSoftReloadAsync();

        /// <summary>
        ///     The .NET generic host instance.
        /// </summary>
        IHost Host { get; }

        /// <summary>
        ///    Information about the OpenMod host.
        /// </summary>
        IHostInformation HostInformation { get; }
    }
}
