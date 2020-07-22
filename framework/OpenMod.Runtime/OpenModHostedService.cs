using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Permissions;
using OpenMod.API.Plugins;
using SmartFormat;

namespace OpenMod.Runtime
{
    [OpenModInternal]
    public class OpenModHostedService : IHostedService
    {
        private readonly ILogger<OpenModHostedService> m_Logger;
        private readonly IPermissionChecker m_PermissionChecker;
        private readonly IRuntime m_Runtime;
        private readonly IOpenModHost m_Host;
        private readonly IPluginAssemblyStore m_PluginAssemblyStore;
        private readonly IPluginActivator m_PluginActivator;

        public OpenModHostedService(
            ILogger<OpenModHostedService> logger,
            IPermissionChecker permissionChecker,
            IRuntime runtime,
            IOpenModHost host,
            IPluginAssemblyStore pluginAssemblyStore,
            IPluginActivator pluginActivator
        )
        {
            m_Logger = logger;
            m_PermissionChecker = permissionChecker;
            m_Runtime = runtime;
            m_Host = host;
            m_PluginAssemblyStore = pluginAssemblyStore;
            m_PluginActivator = pluginActivator;
        }
 
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await m_PermissionChecker.InitAsync();
            Smart.Default.Parser.UseAlternativeEscapeChar('\\');

            m_Logger.LogInformation($"Initializing for host: {m_Host.HostDisplayName} v{m_Host.HostVersion}");
            await m_Host.InitAsync();

            m_Logger.LogInformation("Loading plugins...");

            var i = 0;
            foreach (var pluginAssembly in m_PluginAssemblyStore.LoadedPluginAssemblies)
            {
                if (await m_PluginActivator.TryActivatePluginAsync(pluginAssembly) != null)
                {
                    i++;
                }
            }

            m_Logger.LogInformation($"> {i} plugins loaded.");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}