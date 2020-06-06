using System.Threading.Tasks;

namespace OpenMod.Core.Plugins
{
    public class OpenModUniversalPlugin : OpenModPluginBase
    {
        public sealed override Task LoadAsync()
        {
            return OnLoadAsync();
        }

        protected virtual Task OnLoadAsync()
        {
            return Task.CompletedTask;
        }

        public sealed override Task UnloadAsync()
        {
            return OnUnloadAsync();
        }

        protected virtual Task OnUnloadAsync()
        {
            return Task.CompletedTask;
        }
    }
}