using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace OpenMod.Core.Plugins
{
    public abstract class PluginAssembliesSource
    {
        public abstract Task<ICollection<Assembly>> LoadPluginAssembliesAsync();
    }
}