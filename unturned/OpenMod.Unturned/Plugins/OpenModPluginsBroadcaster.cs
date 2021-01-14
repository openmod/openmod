using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Plugins;
using OpenMod.Core.Eventing;
using OpenMod.Core.Plugins.Events;
using SDG.Unturned;

namespace OpenMod.Unturned.Plugins
{
    [OpenModInternal]
    [UsedImplicitly]
    [EventListenerLifetime(ServiceLifetime.Singleton)]
    public class OpenModPluginsBroadcaster : IEventListener<PluginLoadedEvent>, IEventListener<PluginUnloadedEvent>
    {
        public OpenModPluginsBroadcaster(IPluginActivator pluginActivator)
        {
            var pluginAdvertising = PluginAdvertising.Get();
            pluginAdvertising.PluginFrameworkName = "openmod";

            pluginAdvertising.AddPlugins(
                from plugin in pluginActivator.ActivatedPlugins
                where plugin.IsComponentAlive
                select plugin.DisplayName);
        }

        Task IEventListener<PluginLoadedEvent>.HandleEventAsync(object sender, PluginLoadedEvent @event)
        {
            PluginAdvertising.Get().AddPlugin(@event.Plugin.DisplayName);
            return Task.CompletedTask;
        }

        Task IEventListener<PluginUnloadedEvent>.HandleEventAsync(object sender, PluginUnloadedEvent @event)
        {
            PluginAdvertising.Get().RemovePlugin(@event.Plugin.DisplayName);
            return Task.CompletedTask;
        }
    }
}
