using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenMod.API;
using OpenMod.API.Ioc;
using OpenMod.NuGet;

namespace OpenMod.Runtime
{
    [OpenModInternal]
    public sealed class OpenModStartupContext : IOpenModServiceConfigurationContext
    {
        public IRuntime Runtime { get; internal set; } = null!;
        [Obsolete("Use " + nameof(HostBuilderContext) + "." + nameof(HostBuilderContext.Configuration))]
        public IConfigurationRoot Configuration { get; internal set; } = null!;
        public IOpenModStartup OpenModStartup { get; internal set; } = null!;
        public NuGetPackageManager NuGetPackageManager { get; internal set; } = null!;
        public Dictionary<string, object> DataStore { get; internal set; } = null!;
        public ILoggerFactory LoggerFactory { get; internal set; } = null!;
    }
}