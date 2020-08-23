using OpenMod.API.Plugins;
using OpenMod.Core.Eventing;

namespace OpenMod.Core.Plugins.Events
{
    public class PluginUnloadedEvent : Event
    {
        public IOpenModPlugin Plugin { get; }

        public PluginUnloadedEvent(IOpenModPlugin plugin)
        {
            Plugin = plugin;
        }
    }
}