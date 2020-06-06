using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using OpenMod.API.Ioc;

namespace OpenMod.API.Plugins
{
    [Service]
    public interface IPluginActivator
    {
        IReadOnlyCollection<IOpenModPlugin> ActivatedPlugins { get; }

        Task<IOpenModPlugin> ActivatePluginAsync(Assembly assembly);
    }
}