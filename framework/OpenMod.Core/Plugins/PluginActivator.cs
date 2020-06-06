using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenMod.API.Ioc;
using OpenMod.API.Plugins;
using OpenMod.Core.Helpers;

namespace OpenMod.Core.Plugins
{
    [UsedImplicitly]
    [ServiceImplementation]
    public class PluginActivator : IPluginActivator
    {
        private readonly ILogger<PluginActivator> m_Logger;
        private readonly IServiceProvider m_ServiceProvider;

        public PluginActivator(
            ILogger<PluginActivator> logger,
            IServiceProvider serviceProvider)
        {
            m_Logger = logger;
            m_ServiceProvider = serviceProvider;
        }

        private readonly List<WeakReference> m_ActivatedPlugins = new List<WeakReference>();
        public IReadOnlyCollection<IOpenModPlugin> ActivatedPlugins
        {
            get { return m_ActivatedPlugins.Where(d => d.IsAlive).Select(d => d.Target).Cast<IOpenModPlugin>().ToList(); }
        }

        public async Task<IOpenModPlugin> ActivatePluginAsync(Assembly assembly)
        {
            var pluginTypes = assembly.FindTypes<IOpenModPlugin>(false).ToList();
            if (pluginTypes.Count == 0)
            {
                m_Logger.LogError($"Failed to load plugin from assembly {assembly}: couldn't find any IOpenModPlugin implementation");
                return null;
            }

            if (pluginTypes.Count > 1)
            {
                m_Logger.LogError($"Failed to load plugin from assembly {assembly}: assembly has multiple IOpenModPlugin instances");
                return null;
            }

            var pluginType = pluginTypes.Single();
            IOpenModPlugin pluginInstance;
            try
            {
                pluginInstance = (IOpenModPlugin)ActivatorUtilities.CreateInstance(m_ServiceProvider, pluginType);
            }
            catch (Exception ex)
            {
                m_Logger.LogError(ex, $"Failed to load plugin from type: {pluginType.FullName} in assembly: {assembly.FullName}");
                return null;
            }

            m_Logger.LogInformation($"Loading plugin: {pluginInstance.DisplayName} v{pluginInstance.Version}");

            try
            {
                await pluginInstance.LoadAsync();
            }
            catch (Exception ex)
            {
                m_Logger.LogError(ex, $"Failed to load plugin: {pluginInstance.DisplayName} v{pluginInstance.Version}");
                return null;
            }

            m_ActivatedPlugins.Add(new WeakReference(pluginInstance));
            return pluginInstance;
        }
    }
}