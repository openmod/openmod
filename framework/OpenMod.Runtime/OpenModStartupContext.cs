using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenMod.API;
using OpenMod.API.Ioc;
using OpenMod.NuGet;

namespace OpenMod.Runtime
{
    public sealed class OpenModStartupContext : IOpenModStartupContext
    {
        public IRuntime Runtime { get; internal set; }
        public IConfigurationRoot Configuration { get; internal set; }
        public IOpenModStartup OpenModStartup { get; internal set; }
        public ILoggerFactory LoggerFactory { get; internal set; }
        public NuGetPackageManager NuGetPackageManager { get; internal set; }
        public Dictionary<string, object> DataStore { get; internal set; }
    }
}