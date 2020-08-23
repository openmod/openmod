using OpenMod.API.Plugins;
using OpenMod.Core.Eventing;

namespace OpenMod.Core.Plugins.Events
{
    public class PluginDisposeEvent : Event
    {
        public IOpenModPlugin Plugin { get; }

        public PluginDisposeEvent(IOpenModPlugin plugin)
        {
            Plugin = plugin;
        }
    }
}