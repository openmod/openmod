using Autofac;
using Microsoft.Extensions.Configuration;

namespace OpenMod.API.Plugins
{
    public interface IPluginServiceConfigurationContext
    {
        ILifetimeScope ParentLifetimeScope { get; }
        IConfiguration Configuration { get; }
        ContainerBuilder ContainerBuilder { get; }
    }
}