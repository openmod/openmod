using System;
using System.Threading.Tasks;

namespace OpenMod.Core.Plugins
{
    public class OpenModUniversalPlugin : OpenModPluginBase
    {
        public OpenModUniversalPlugin(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public sealed override Task LoadAsync()
        {
            base.LoadAsync();
            return OnLoadAsync();
        }

        protected virtual Task OnLoadAsync()
        {
            return Task.CompletedTask;
        }

        public sealed override Task UnloadAsync()
        {
            base.UnloadAsync();
            return OnUnloadAsync();
        }

        protected virtual Task OnUnloadAsync()
        {
            return Task.CompletedTask;
        }
    }
}