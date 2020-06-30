using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using OpenMod.Core.Plugins;

namespace OpenMod.UnityEngine.Plugins
{
    public abstract class OpenModUnityEnginePlugin : OpenModPluginBase
    {
        protected OpenModUnityEnginePlugin(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            
        }

        public sealed override async Task LoadAsync()
        {
            await base.LoadAsync();
            await OnLoadAsync();
        }

        protected virtual UniTask OnLoadAsync()
        {
            return UniTask.CompletedTask;
        }

        public sealed override async Task UnloadAsync()
        {
            await base.UnloadAsync();
            await OnUnloadAsync();
        }

        protected virtual UniTask OnUnloadAsync()
        {
            return UniTask.CompletedTask;
        }
    }
}