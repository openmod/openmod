using OpenMod.API.Plugins;
using OpenMod.Core.Eventing;

namespace OpenMod.Core.Plugins.Events
{
    public class PluginLoadedEvent : Event
    {
        public IOpenModPlugin Plugin { get; }

        public PluginLoadedEvent(IOpenModPlugin plugin)
        {
            Plugin = plugin;
        }
    }
}