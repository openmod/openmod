using System.Linq;
using JetBrains.Annotations;
using OpenMod.API.Ioc;
using OpenMod.API.Plugins;
using OpenMod.API.Prioritization;

namespace OpenMod.Core.Plugins
{
    [UsedImplicitly]
    [ServiceImplementation(Priority = Priority.Lowest)]
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