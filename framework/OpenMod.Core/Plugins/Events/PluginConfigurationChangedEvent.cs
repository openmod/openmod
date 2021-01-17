using Microsoft.Extensions.Configuration;
using OpenMod.API.Plugins;
using OpenMod.Core.Eventing;

namespace OpenMod.Core.Plugins.Events
{
    public class PluginConfigurationChangedEvent : Event
    {
        public IOpenModPlugin Plugin { get; }
        public IConfiguration Configuration { get; }

        public PluginConfigurationChangedEvent(IOpenModPlugin plugin, IConfiguration configuration)
        {
            Plugin = plugin;
            Configuration = configuration;
        }
    }
}