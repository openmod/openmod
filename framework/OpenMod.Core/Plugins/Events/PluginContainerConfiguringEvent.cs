using System;
using Autofac;
using Microsoft.Extensions.Configuration;
using OpenMod.API.Plugins;
using OpenMod.Core.Eventing;

namespace OpenMod.Core.Plugins.Events
{
    public class PluginContainerConfiguringEvent : Event
    {
        public PluginMetadataAttribute Metadata { get; }
        public Type PluginType { get; }
        public IConfiguration Configuration { get; }
        public ContainerBuilder ContainerBuilder { get; }
        public string WorkingDirectory { get; }

        public PluginContainerConfiguringEvent(PluginMetadataAttribute metadata, 
            Type pluginType, 
            IConfiguration configuration, 
            ContainerBuilder containerBuilder,
            string workingDirectory)
        {
            Metadata = metadata;
            PluginType = pluginType;
            Configuration = configuration;
            ContainerBuilder = containerBuilder;
            WorkingDirectory = workingDirectory;
        }
    }
}