using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace OpenMod.Core.Plugins
{
    public abstract class PluginProvider
    {
        public abstract Task<ICollection<Assembly>> LoadPluginAssembliesAsync();
    }
}