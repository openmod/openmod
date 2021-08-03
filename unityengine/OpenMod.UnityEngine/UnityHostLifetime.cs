using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenMod.API;
using OpenMod.Core.Helpers;
using OpenMod.UnityEngine.Helpers;
using UnityEngine;
using UnityEngine.LowLevel;

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
            if (!PlayerLoopHelper.IsInjectedUniTaskPlayerLoop())
            {
                var unitySynchronizationContextField =
                    typeof(PlayerLoopHelper).GetField("unitySynchronizationContext",
                        BindingFlags.Static | BindingFlags.NonPublic);

                // For older version of UniTask
                unitySynchronizationContextField ??=
                    typeof(PlayerLoopHelper).GetField("unitySynchronizationContetext",
                        BindingFlags.Static | BindingFlags.NonPublic)
                    ?? throw new Exception("Could not find PlayerLoopHelper.unitySynchronizationContext field");

                unitySynchronizationContextField.SetValue(null, SynchronizationContext.Current);

                var mainThreadIdField =
                    typeof(PlayerLoopHelper).GetField("mainThreadId", BindingFlags.Static | BindingFlags.NonPublic)
                    ?? throw new Exception("Could not find PlayerLoopHelper.mainThreadId field");
                mainThreadIdField.SetValue(null, Thread.CurrentThread.ManagedThreadId);

                var playerLoop = PlayerLoop.GetCurrentPlayerLoop();
                PlayerLoopHelper.Initialize(ref playerLoop);
            }

            // Handle UniTask exception
            UniTaskScheduler.UnobservedTaskException += UniTaskExceptionHandler;
            // Do not switch thread
            UniTaskScheduler.DispatchUnityMainThread = false;

            TlsWorkaround.Install();

            Application.quitting += OnApplicationQuitting;
            Console.CancelKeyPress += OnCancelKeyPress;
            return Task.CompletedTask;
        }

        private void UniTaskExceptionHandler(Exception exception)
        {
            m_Logger.LogError(exception, "Caught UnobservedTaskException");
        }

        private void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
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

            TlsWorkaround.Uninstall();

            UniTaskScheduler.UnobservedTaskException -= UniTaskExceptionHandler;
            UniTaskScheduler.DispatchUnityMainThread = true;

            Application.quitting -= OnApplicationQuitting;
            Console.CancelKeyPress -= OnCancelKeyPress;
        }
    }
}