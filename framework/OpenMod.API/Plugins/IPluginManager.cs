using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace OpenMod.API.Plugins
{
    // this is not a service because the IoC container does not exist yet when it is used
    public interface IPluginAssemblyStore
    {
        Task LoadPluginAssembliesAsync();
        IReadOnlyCollection<Assembly> LoadedPluginAssemblies { get; }
    }
}