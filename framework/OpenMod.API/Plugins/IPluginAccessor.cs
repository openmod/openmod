using JetBrains.Annotations;
using OpenMod.API.Ioc;

namespace OpenMod.API.Plugins
{
    [Service]
    public interface IPluginAccessor<out TPlugin> where TPlugin: IOpenModPlugin
    {
        [CanBeNull]
        TPlugin Instance { get; }
    }
}