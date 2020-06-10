using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenMod.API;
using OpenMod.API.Permissions;
using OpenMod.API.Plugins;
using SmartFormat;

namespace OpenMod.Runtime
{
    public class OpenModHostedService : BackgroundService
    {
        private readonly ILogger<OpenModHostedService> m_Logger;
        private readonly IPermissionChecker m_PermissionChecker;
        private readonly IOpenModHost m_Host;
        private readonly IPluginAssemblyStore m_PluginAssemblyStore;
        private readonly IPluginActivator m_PluginActivator;

        public OpenModHostedService(
            ILogger<OpenModHostedService> logger,
            IPermissionChecker permissionChecker,
            IOpenModHost host,
            IPluginAssemblyStore pluginAssemblyStore,
            IPluginActivator pluginActivator
        )
        {
            m_Logger = logger;
            m_PermissionChecker = permissionChecker;
            m_Host = host;
            m_PluginAssemblyStore = pluginAssemblyStore;
            m_PluginActivator = pluginActivator;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await m_PermissionChecker.InitAsync();
            Smart.Default.Parser.UseAlternativeEscapeChar('\\');

            m_Logger.LogInformation($"Initializing for host: {m_Host.DisplayName} v{m_Host.Version}");
            await m_Host.InitAsync();
            m_Logger.LogInformation("OpenMod has been initialized.");

            m_Logger.LogInformation("Loading plugins...");
            foreach (var pluginAssembly in m_PluginAssemblyStore.LoadedPluginAssemblies)
            {
                await m_PluginActivator.TryActivatePluginAsync(pluginAssembly);
            }

            m_Logger.LogInformation("Plugins loaded.");
        }
    }
}