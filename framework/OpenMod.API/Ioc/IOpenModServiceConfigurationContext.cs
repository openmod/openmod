using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace OpenMod.API.Ioc
{
    public interface IOpenModServiceConfigurationContext
    {
        IRuntime Runtime { get; }
        IConfigurationRoot Configuration { get; }
        IOpenModStartup OpenModStartup { get; }
        Dictionary<string, object> DataStore { get; }
        ILoggerFactory LoggerFactory { get; }
    }
}