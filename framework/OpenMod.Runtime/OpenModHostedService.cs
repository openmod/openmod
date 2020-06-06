using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenMod.API;

namespace OpenMod.Runtime
{
    public class OpenModHostedService : BackgroundService
    {
        private readonly ILogger<OpenModHostedService> m_Logger;
        //private readonly IPermissionStore m_PermissionProvider;
        private readonly IOpenModHost m_Host;

        public OpenModHostedService(
            ILogger<OpenModHostedService> logger,
            //IPermissionStore permissionProvider,
            IOpenModHost host
        )
        {
            m_Logger = logger;
            //m_PermissionProvider = permissionProvider;
            m_Host = host;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //await m_PermissionProvider.LoadAsync();
            m_Logger.LogInformation($"Initializing for host: {m_Host.Name} v{m_Host.Version}");
            await m_Host.InitAsync();
            m_Logger.LogInformation("OpenMod has been initialized.");
        }
    }
}