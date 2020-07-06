using System;
using Autofac;
using Microsoft.Extensions.Configuration;

namespace OpenMod.API.Plugins
{
    public interface IPluginContainerConfigurator 
    {
        void ConfigureContainer(ILifetimeScope parentLifetimeScope, IConfiguration configuration, ContainerBuilder containerBuilder);
    }
}