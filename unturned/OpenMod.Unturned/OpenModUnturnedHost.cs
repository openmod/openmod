using Autofac;
using Cysharp.Threading.Tasks;
using HarmonyLib;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenMod.API;
using OpenMod.API.Commands;
using OpenMod.API.Ioc;
using OpenMod.API.Persistence;
using OpenMod.Core.Console;
using OpenMod.Core.Helpers;
using OpenMod.Extensions.Games.Abstractions;
using OpenMod.Unturned.Events;
using OpenMod.Unturned.Helpers;
using OpenMod.Unturned.Logging;
using OpenMod.Unturned.Users;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using OpenMod.Core.Ioc;
using OpenMod.NuGet;
using OpenMod.Unturned.RocketMod;
using UnityEngine.LowLevel;
using Priority = OpenMod.API.Prioritization.Priority;

namespace OpenMod.Unturned
{
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    public class OpenModUnturnedHost : IOpenModHost, IDisposable
    {
        private static bool s_UniTaskInited;
        private readonly IRuntime m_Runtime;
        private readonly IHostInformation m_HostInformation;
        private readonly IServiceProvider m_ServiceProvider;
        private readonly IConsoleActorAccessor m_ConsoleActorAccessor;
        private readonly Lazy<ICommandExecutor> m_CommandExecutor;
        private readonly ILogger<OpenModUnturnedHost> m_Logger;
        private readonly NuGetPackageManager m_NuGetPackageManager;
        private readonly Lazy<UnturnedCommandHandler> m_UnturnedCommandHandler;
        private readonly HashSet<string> m_Capabilities;
        private OpenModConsoleInputOutput? m_OpenModIoHandler;
        private List<ICommandInputOutput>? m_IoHandlers;
        private UnturnedEventsActivator? m_UnturnedEventsActivator;
        private Harmony? m_Harmony;
        private bool m_IsDisposing;

        public string OpenModComponentId { get; } = "OpenMod.Unturned";

        public string WorkingDirectory { get; }

        public bool IsComponentAlive { get; private set; }

        public ILifetimeScope LifetimeScope { get; }

        public IDataStore DataStore { get; }

        public OpenModUnturnedHost(
            IRuntime runtime,
            IHostInformation hostInformation,
            IServiceProvider serviceProvider,
            ILifetimeScope lifetimeScope,
            IDataStoreFactory dataStoreFactory,
            IConsoleActorAccessor consoleActorAccessor,
            ILogger<OpenModUnturnedHost> logger,
            NuGetPackageManager nuGetPackageManager,
            Lazy<ICommandExecutor> commandExecutor,
            Lazy<UnturnedCommandHandler> unturnedCommandHandler)
        {
            m_Runtime = runtime;
            m_HostInformation = hostInformation;
            m_ServiceProvider = serviceProvider;
            m_ConsoleActorAccessor = consoleActorAccessor;
            m_CommandExecutor = commandExecutor;
            m_Logger = logger;
            m_NuGetPackageManager = nuGetPackageManager;
            m_UnturnedCommandHandler = unturnedCommandHandler;
            WorkingDirectory = runtime.WorkingDirectory;
            LifetimeScope = lifetimeScope;

            DataStore = dataStoreFactory.CreateDataStore(new DataStoreCreationParameters
            {
                Component = this,
                Prefix = "openmod.unturned",
                Suffix = null,
                WorkingDirectory = WorkingDirectory
            });

            m_Capabilities = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                KnownGameCapabilities.Health,
                KnownGameCapabilities.Inventory,
                KnownGameCapabilities.Vehicles
            };
        }

        public bool HasCapability(string capability)
        {
            return m_Capabilities.Contains(capability);
        }

        public Task InitAsync()
        {
            try
            {
                if (RocketModIntegration.IsRocketModInstalled())
                {
                    var rocketModIntegration = ActivatorUtilitiesEx.CreateInstance<RocketModIntegration>(LifetimeScope);
                    rocketModIntegration.Install();
                }
            }
            catch (Exception ex)
            {
                m_Logger.LogError(ex, "Failed to integrate with RocketMod");
            }

            // ReSharper disable PossibleNullReferenceException
            IsComponentAlive = true;

            m_Harmony = new Harmony(OpenModComponentId);
            m_Harmony.PatchAll(GetType().Assembly);

            m_UnturnedCommandHandler.Value.Subscribe();
            BindUnturnedEvents();

            var ioHandlers = (List<ICommandInputOutput>)typeof(CommandWindow)
                .GetField("ioHandlers", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(Dedicator.commandWindow);

            CommandLineFlag? shouldManageConsole = null;
            var previousShouldManageConsoleValue = true;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                /* Fix Unturned destroying console and breaking Serilog formatting and colors */
                var windowsConsole = typeof(Provider).Assembly.GetType("SDG.Unturned.WindowsConsole");
                var shouldManageConsoleField = windowsConsole?.GetField("shouldManageConsole",
                    BindingFlags.Static | BindingFlags.NonPublic);

                if (shouldManageConsoleField != null)
                {
                    shouldManageConsole = (CommandLineFlag)shouldManageConsoleField.GetValue(null);
                    previousShouldManageConsoleValue = shouldManageConsole.value;
                    shouldManageConsole.value = false;
                }
            }

            m_IoHandlers = ioHandlers.ToList(); // copy Unturneds IoHandlers
            // unturned built-in io handlers
            var defaultIoHandlers = ioHandlers.Where(c =>
                c.GetType().FullName
                    .Equals("SDG.Unturned.ThreadedWindowsConsoleInputOutput") // type doesnt exist on Linux
                || c.GetType().FullName.Equals("SDG.Unturned.WindowsConsoleInputOutput") // type doesnt exist on Linux
                || c.GetType() == typeof(ThreadedConsoleInputOutput)
                || c.GetType() == typeof(ConsoleInputOutput)).ToList();

            foreach (var ioHandler in defaultIoHandlers)
            {
                Dedicator.commandWindow.removeIOHandler(ioHandler);
            }

            if (shouldManageConsole != null)
            {
                shouldManageConsole.value = previousShouldManageConsoleValue;
            }

            m_OpenModIoHandler = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? ActivatorUtilities.CreateInstance<OpenModWindowsConsoleInputOutput>(m_ServiceProvider)
                : ActivatorUtilities.CreateInstance<OpenModConsoleInputOutput>(m_ServiceProvider);

            Dedicator.commandWindow.addIOHandler(m_OpenModIoHandler);

            m_Logger.LogInformation("OpenMod for Unturned v{HostVersion} is initializing...",
                m_HostInformation.HostVersion);

            TlsWorkaround.Install();

            if (!s_UniTaskInited)
            {
                var unitySynchronizationContetextField =
                    typeof(PlayerLoopHelper).GetField("unitySynchronizationContetext",
                        BindingFlags.Static | BindingFlags.NonPublic);
                unitySynchronizationContetextField.SetValue(null, SynchronizationContext.Current);

                var mainThreadIdField =
                    typeof(PlayerLoopHelper).GetField("mainThreadId", BindingFlags.Static | BindingFlags.NonPublic);
                mainThreadIdField.SetValue(null, Thread.CurrentThread.ManagedThreadId);

                var playerLoop = PlayerLoop.GetDefaultPlayerLoop();
                PlayerLoopHelper.Initialize(ref playerLoop);
                s_UniTaskInited = true;
            }

            m_Logger.LogInformation("OpenMod for Unturned is ready");

            return Task.CompletedTask;
            // ReSharper restore PossibleNullReferenceException
        }

        public async Task PerformHardReloadAsync()
        {
            var shutdownPerformed = false;
            try
            {
                await m_Runtime.ShutdownAsync();
                shutdownPerformed = true;
                m_NuGetPackageManager.ClearCache();

                var bootstrapperAssembly = AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(d => d.GetName().Name.Equals("OpenMod.Unturned.Module.Bootstrapper"));

                var bootstrapperClass = bootstrapperAssembly!.GetType("OpenMod.Unturned.Module.Bootstrapper.BootstrapperModule");
                var instanceProperty = bootstrapperClass.GetProperty("Instance", BindingFlags.Public |  BindingFlags.Static);
                var initializeMethod = bootstrapperClass.GetMethod("initialize", BindingFlags.Instance | BindingFlags.Public);
                var moduleInstance = instanceProperty!.GetValue(null);

                initializeMethod!.Invoke(moduleInstance, new object[0]);
            }
            catch (Exception ex)
            {
                if (shutdownPerformed)
                {
                    // fallback to unturned log because our logger is disposed at this point
                    UnturnedLog.exception(ex, "OpenMod has crashed.");
                }

                throw;
            }
        }

        protected virtual void BindUnturnedEvents()
        {
            CommandWindow.onCommandWindowInputted += OnCommandWindowInputted;

            m_UnturnedEventsActivator = ActivatorUtilities.CreateInstance<UnturnedEventsActivator>(m_ServiceProvider);
            m_UnturnedEventsActivator.ActivateEventListeners();
        }

        protected virtual void UnbindUnturnedEvents()
        {
            CommandWindow.onCommandWindowInputted -= OnCommandWindowInputted;
            m_UnturnedEventsActivator?.Dispose();
        }

        // ReSharper disable once RedundantAssignment
        private void OnCommandWindowInputted(string text, ref bool shouldExecuteCommand)
        {
            shouldExecuteCommand = false;

            var actor = m_ConsoleActorAccessor.Actor;
            var args = ArgumentsParser.ParseArguments(text);
            if (args.Length == 0)
            {
                return;
            }

            AsyncHelper.Schedule("Console command execution",
                () => m_CommandExecutor.Value.ExecuteAsync(actor, args, string.Empty));
        }

        public Task ShutdownAsync()
        {
            async UniTask ShutdownTask()
            {
                await UniTask.SwitchToMainThread();
                Provider.shutdown();
            }

            return ShutdownTask().AsTask();
        }

        public void Dispose()
        {
            if (m_IsDisposing)
            {
                return;
            }

            Dedicator.commandWindow.removeIOHandler(m_OpenModIoHandler);

            if (m_IoHandlers is not null)
            {
                m_IoHandlers.Reverse();
                foreach (var ioHandler in m_IoHandlers)
                {
                    Dedicator.commandWindow.addIOHandler(ioHandler);
                }

                m_IoHandlers.Clear();
            }

            IsComponentAlive = false;
            m_IsDisposing = true;
            TlsWorkaround.Uninstalll();

            m_Harmony?.UnpatchAll(OpenModComponentId);
            UnbindUnturnedEvents();
        }
    }
}