using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenMod.API.Persistence;

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
        /// The OpenMod data store. See <see cref="IDataStore"/>.
        /// </summary>
        Dictionary<string, object> DataStore { get; }

        /// <summary>
        /// The logger factory.
        /// </summary>
        ILoggerFactory LoggerFactory { get; }
    }
}