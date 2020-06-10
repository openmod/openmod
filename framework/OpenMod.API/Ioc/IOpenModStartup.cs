using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using OpenMod.API.Plugins;

namespace OpenMod.API.Ioc
{
    public interface IOpenModStartup
    {
        IOpenModStartupContext Context { get; }

        void RegisterIocAssemblyAndCopyResources(Assembly assembly, string assemblyDir);

        Task<ICollection<Assembly>> RegisterPluginAssembliesAsync(IPluginAssembliesSource source);
    }
}