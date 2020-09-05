using OpenMod.API.Plugins;
using OpenMod.Core.Eventing;

namespace OpenMod.Core.Plugins.Events
{
    public class PluginDisposedEvent : Event
    {
        public IOpenModPlugin Plugin { get; }

        public PluginDisposedEvent(IOpenModPlugin plugin)
        {
            Plugin = plugin;
        }
    }
}