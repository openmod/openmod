using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Hosting;
using Semver;

namespace OpenMod.API
{
    /// <summary>
    ///     Defines the OpenMod Runtime. This class is responsible for initializing OpenMod.
    /// </summary>
    public interface IRuntime
    {
        /// <summary>
        ///     Initializes the runtime.
        /// </summary>
        /// <returns></returns>
        Task InitAsync(ICollection<Assembly> openModHostAssemblies, IHostBuilder hostBuilder, RuntimeInitParameters parameters);

        /// <summary>
        ///     Shuts down OpenMod and disposes all services.
        /// </summary>
        Task ShutdownAsync();

        /// <summary>
        ///     Gets the OpenMod runtime version.
        /// </summary>
        SemVersion Version { get; }
    }

    public struct RuntimeInitParameters
    {
        public string WorkingDirectory;
        public string[] CommandlineArgs;
    }
}
