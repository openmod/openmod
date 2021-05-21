using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenMod.API;
using OpenMod.Core.Helpers;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace OpenMod.UnityEngine
{
    public class UnityHostLifetime : IHostLifetime, IDisposable
    {
        private readonly IHostApplicationLifetime m_ApplicationLifetime;
        private readonly ILogger<UnityHostLifetime> m_Logger;
        private readonly IOpenModHost m_OpenModHost;
        private readonly HostOptions m_HostOptions;
        private readonly ManualResetEvent m_ShutdownBlock;

        public UnityHostLifetime(IHostApplicationLifetime applicationLifetime, ILogger<UnityHostLifetime> logger,
            IOptions<HostOptions> hostOptions, IOpenModHost openModHost)
        {
            m_ApplicationLifetime = applicationLifetime;
            m_Logger = logger;
            m_OpenModHost = openModHost;
            m_HostOptions = hostOptions.Value;
            m_ShutdownBlock = new ManualResetEvent(false);
        }

        public Task WaitForStartAsync(CancellationToken cancellationToken)
        {
            Application.quitting += OnApplicationQuitting;
            Console.CancelKeyPress += Console_CancelKeyPress;
            return Task.CompletedTask;
        }

        private void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            AsyncHelper.RunSync(m_OpenModHost.ShutdownAsync);
        }

        private void OnApplicationQuitting()
        {
            m_ApplicationLifetime.StopApplication();

            if (!m_ShutdownBlock.WaitOne(m_HostOptions.ShutdownTimeout))
            {
                m_Logger.LogInformation("Waiting for the host to be disposed. Ensure all 'IHost' instances are wrapped in 'using' blocks");
            }

            m_ShutdownBlock.WaitOne();

            // On Linux if the shutdown is triggered by SIGTERM then that's signaled with the 143 exit code.
            // Suppress that since we shut down gracefully. https://github.com/aspnet/AspNetCore/issues/6526
            Environment.ExitCode = 0;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // There's nothing to do here
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            m_ShutdownBlock.Set();
            Application.quitting -= OnApplicationQuitting;
            Console.CancelKeyPress -= Console_CancelKeyPress;
        }
    }
}