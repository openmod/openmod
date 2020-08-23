using System;
using System.Threading.Tasks;
using OpenMod.API;
using OpenMod.Core.Plugins.Events;

namespace OpenMod.Core.Plugins
{
    public class OpenModUniversalPlugin : OpenModPluginBase
    {
        public OpenModUniversalPlugin(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [OpenModInternal]
        public sealed override async Task LoadAsync()
        {
            var @event = new PluginLoadEvent(this);
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
            await EventBus.EmitAsync(this, this, new PluginUnloadEvent(this));

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