using System.Reflection;
using System.Threading.Tasks;
using OpenMod.API.Plugins;

namespace OpenMod.API.Ioc
{
    public interface IOpenModStartup
    {
        void RegisterServiceFromAssemblyWithResources(Assembly assembly, string relativeDir);

        Task RegisterPluginAssembliesAsync(IPluginAssembliesSource source);
    }
}