using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace OpenMod.API.Plugins
{
    public interface IPluginAssembliesSource
    {
        Task<ICollection<Assembly>> LoadPluginAssembliesAsync();
    }
}