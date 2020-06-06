using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using OpenMod.Core.Plugins;

namespace OpenMod.Unturned.Plugins
{
    public abstract class OpenModUnturnedPlugin : OpenModPluginBase
    {
        public sealed override async Task LoadAsync()
        {
            await OnLoadAsync();
        }

        protected virtual UniTask OnLoadAsync()
        {
            return UniTask.CompletedTask;
        }

        public sealed override async Task UnloadAsync()
        {
            await OnUnloadAsync();
        }

        protected virtual UniTask OnUnloadAsync()
        {
            return UniTask.CompletedTask;
        }
    }
}