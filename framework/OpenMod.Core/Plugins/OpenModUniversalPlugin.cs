using System;
using System.Threading.Tasks;
using OpenMod.API;
using OpenMod.Core.Plugins.Events;

namespace OpenMod.Core.Plugins
{
    /// <summary>
    /// Base class for all OpenMod universal plugins.
    /// </summary>
    public abstract class OpenModUniversalPlugin : OpenModPluginBase
    {
        protected OpenModUniversalPlugin(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [OpenModInternal]
        public sealed override async Task LoadAsync()
        {
            var @event = new PluginLoadingEvent(this);
            await EventBus.EmitAsync(this, this, @event);

            if (@event.IsCancelled)
            {
                return;
            }

            await base.LoadAsync();
            await OnLoadAsync();

            await EventBus.EmitAsync(this, this, new PluginLoadedEvent(this));
        }

        protected virtual Task OnLoadAsync()
        {
            return Task.CompletedTask;
        }

        [OpenModInternal]
        public sealed override async Task UnloadAsync()
        {
            await EventBus.EmitAsync(this, this, new PluginUnloadingEvent(this));

            await base.UnloadAsync();
            await OnUnloadAsync();

            await EventBus.EmitAsync(this, this, new PluginUnloadedEvent(this));
        }

        protected virtual Task OnUnloadAsync()
        {
            return Task.CompletedTask;
        }
    }
}