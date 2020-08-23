using OpenMod.API.Plugins;
using OpenMod.Core.Eventing;

namespace OpenMod.Core.Plugins.Events
{
    public class PluginUnloadEvent : Event
    {
        public IOpenModPlugin Plugin { get; }

        public PluginUnloadEvent(IOpenModPlugin plugin)
        {
            Plugin = plugin;
        }
    }
}