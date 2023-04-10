using Autofac;
using Microsoft.Extensions.Configuration;
using OpenMod.API.Plugins;

namespace OpenMod.Core.Plugins
{
    public class PluginServiceConfigurationContext : IPluginServiceConfigurationContext
    {
        public PluginServiceConfigurationContext(ILifetimeScope parentLifetimeScope, IConfigurationRoot configuration,
            ContainerBuilder containerBuilder, string workingDirectory)
        {
            ParentLifetimeScope = parentLifetimeScope;
            Configuration = configuration;
            ContainerBuilder = containerBuilder;
            WorkingDirectory = workingDirectory;
        }

        public ILifetimeScope ParentLifetimeScope { get; }

        public IConfiguration Configuration { get; }

        public ContainerBuilder ContainerBuilder { get; }

        public string WorkingDirectory { get; }
    }
}