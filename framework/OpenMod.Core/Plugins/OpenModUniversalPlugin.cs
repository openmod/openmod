using System;
using System.Threading.Tasks;

namespace OpenMod.Core.Plugins
{
    public class OpenModUniversalPlugin : OpenModPluginBase
    {
        public OpenModUniversalPlugin(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public sealed override async Task LoadAsync()
        {
            await base.LoadAsync();
            await OnLoadAsync();
        }

        protected virtual Task OnLoadAsync()
        {
            return Task.CompletedTask;
        }

        public sealed override async Task UnloadAsync()
        {
            await base.UnloadAsync();
            await OnUnloadAsync();
        }

        protected virtual Task OnUnloadAsync()
        {
            return Task.CompletedTask;
        }
    }
}