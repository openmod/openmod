using OpenMod.API.Plugins;
using OpenMod.Core.Eventing;

namespace OpenMod.Core.Plugins.Events
{
    public class PluginUnloadingEvent : Event
    {
        public IOpenModPlugin Plugin { get; }

        public PluginUnloadingEvent(IOpenModPlugin plugin)
        {
            Plugin = plugin;
        }
    }
}