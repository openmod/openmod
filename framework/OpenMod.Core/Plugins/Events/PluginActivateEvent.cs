using OpenMod.API.Eventing;
using OpenMod.API.Plugins;
using OpenMod.Core.Eventing;

namespace OpenMod.Core.Plugins.Events
{
    public class PluginActivateEvent : Event, ICancellableEvent
    {
        public IOpenModPlugin Plugin { get; }

        public PluginActivateEvent(IOpenModPlugin plugin)
        {
            Plugin = plugin;
        }

        public bool IsCancelled { get; set; }
    }
}