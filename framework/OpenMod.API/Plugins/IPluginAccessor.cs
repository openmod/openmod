using JetBrains.Annotations;

namespace OpenMod.API.Plugins
{
    public interface IPluginAccessor<out TPlugin> where TPlugin: IOpenModPlugin
    {
        [CanBeNull]
        TPlugin Instance { get; }
    }
}