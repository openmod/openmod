using OpenMod.API.Plugins;
using OpenMod.Core.Eventing;

namespace OpenMod.Core.Plugins
{
    public sealed class OpenModPluginsLoadedEvent : Event
    {
        public IPluginAssemblyStore AssemblyStore { get; }

        public OpenModPluginsLoadedEvent(IPluginAssemblyStore assemblyStore)
        {
            AssemblyStore = assemblyStore;
        }
    }
}