using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace OpenMod.API.Ioc
{
    /// <summary>
    /// The context used when the OpenMod container or configuration gets built.
    /// </summary>
    public interface IOpenModServiceConfigurationContext
    {
        /// <summary>
        /// The OpenMod runtime
        /// </summary>
        IRuntime Runtime { get; }

        /// <summary>
        /// The current OpenMod configuration.
        /// </summary>
        IConfigurationRoot Configuration { get; }

        /// <summary>
        /// The OpenMod startup utility instance.
        /// </summary>
        IOpenModStartup OpenModStartup { get; }

        /// <summary>
        /// Data store for startup.
        /// </summary>
        Dictionary<string, object> DataStore { get; }

        /// <summary>
        /// The logger factory.
        /// </summary>
        ILoggerFactory LoggerFactory { get; }
    }
}