using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Plugins;
using OpenMod.Core.Eventing;
using OpenMod.Core.Plugins.Events;
using OpenMod.Unturned.Configuration;
using SDG.Unturned;

namespace OpenMod.Unturned.Plugins
{
    /// <summary>
    /// Adds plugins to and removes them from Unturned's plugin advertising.
    /// </summary>
    [OpenModInternal]
    [UsedImplicitly]
    [EventListenerLifetime(ServiceLifetime.Singleton)]
    public class OpenModPluginsBroadcaster : IEventListener<PluginLoadedEvent>, IEventListener<PluginUnloadedEvent>
    {
        private readonly IOpenModUnturnedConfiguration m_UnturnedConfiguration;

        public OpenModPluginsBroadcaster(
            IPluginActivator pluginActivator, 
            IOpenModUnturnedConfiguration unturnedConfiguration)
        {
            m_UnturnedConfiguration = unturnedConfiguration;
            var pluginAdvertising = PluginAdvertising.Get();
            pluginAdvertising.PluginFrameworkName = "openmod";

            pluginAdvertising.AddPlugins(
                from plugin in pluginActivator.ActivatedPlugins
                where plugin.IsComponentAlive
                select plugin.DisplayName);
        }

        Task IEventListener<PluginLoadedEvent>.HandleEventAsync(object? sender, PluginLoadedEvent @event)
        {
            var advertisePlugins = m_UnturnedConfiguration.Configuration.GetSection("advertisePlugins").Get<bool>();
            if (advertisePlugins)
            {
                PluginAdvertising.Get().AddPlugin(@event.Plugin.DisplayName);
            }

            return Task.CompletedTask;
        }

        Task IEventListener<PluginUnloadedEvent>.HandleEventAsync(object? sender, PluginUnloadedEvent @event)
        {
            PluginAdvertising.Get().RemovePlugin(@event.Plugin.DisplayName);
            return Task.CompletedTask;
        }
    }
}
