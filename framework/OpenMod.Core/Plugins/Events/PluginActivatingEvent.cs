using OpenMod.API.Eventing;
using OpenMod.API.Plugins;
using OpenMod.Core.Eventing;

namespace OpenMod.Core.Plugins.Events
{
    public class PluginActivatingEvent : Event, ICancellableEvent
    {
        public IOpenModPlugin Plugin { get; }

        public PluginActivatingEvent(IOpenModPlugin plugin)
        {
            Plugin = plugin;
        }

        public bool IsCancelled { get; set; }
    }
}