using System.Linq;
using JetBrains.Annotations;
using OpenMod.API.Plugins;

namespace OpenMod.Core.Plugins
{
    [UsedImplicitly]
    public class PluginAccessor<TPlugin> : IPluginAccessor<TPlugin> where TPlugin : IOpenModPlugin
    {
        private readonly IPluginActivator m_PluginActivator;

        public PluginAccessor(IPluginActivator pluginActivator)
        {
            m_PluginActivator = pluginActivator;
        }
        public TPlugin Instance
        {
            get
            {
                return (TPlugin) m_PluginActivator.ActivatedPlugins.FirstOrDefault(d => d.GetType() == typeof(TPlugin));
            }
        }
    }
}