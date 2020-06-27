using System;
using System.Threading.Tasks;
using OpenMod.Core.Plugins;

namespace OpenMod.UnityEngine.Plugins
{
    public abstract class OpenModUnityEnginePlugin : OpenModPluginBase
    {
        protected OpenModUnityEnginePlugin(IServiceProvider serviceProvider) : base(serviceProvider)
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