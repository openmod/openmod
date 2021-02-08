using OpenMod.API.Eventing;
using OpenMod.API.Plugins;
using OpenMod.Core.Eventing;

namespace OpenMod.Core.Plugins.Events
{
    public class PluginLoadingEvent : Event, ICancellableEvent
    {
        public IOpenModPlugin Plugin { get; }

        public PluginLoadingEvent(IOpenModPlugin plugin)
        {
            Plugin = plugin;
        }

        public bool IsCancelled { get; set; }
    }
}