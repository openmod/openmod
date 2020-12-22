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
using OpenMod.API.Plugins;
using OpenMod.Unturned.RocketMod;
using UnityEngine.LowLevel;
using Priority = OpenMod.API.Prioritization.Priority;

namespace OpenMod.Unturned
{
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    public class OpenModUnturnedHost : IOpenModHost, IDisposable
    {
        private readonly IHostInformation m_HostInformation;
        private readonly IServiceProvider m_ServiceProvider;
        private readonly IConsoleActorAccessor m_ConsoleActorAccessor;
        private readonly ICommandExecutor m_CommandExecutor;
        private readonly ILogger<OpenModUnturnedHost> m_Logger;
        private readonly IPluginActivator m_PluginActivator;
        private readonly UnturnedCommandHandler m_UnturnedCommandHandler;
        private List<ICommandInputOutput> m_IoHandlers;
        private OpenModConsoleInputOutput m_OpenModIoHandler;
        private readonly HashSet<string> m_Capabilities;
        private UnturnedEventsActivator m_UnturnedEventsActivator;
        private Harmony m_Harmony;

        public string OpenModComponentId { get; } = "OpenMod.Unturned";

        public string WorkingDirectory { get; }

        public bool IsComponentAlive { get; private set; }

        public ILifetimeScope LifetimeScope { get; }

        public IDataStore DataStore { get; }

        private bool m_IsDisposing;

        private static bool s_UniTaskInited;

        // ReSharper disable once SuggestBaseTypeForParameter /* we don't want this because of DI */

        public OpenModUnturnedHost(
            IRuntime runtime,
            IHostInformation hostInformation,
            IServiceProvider serviceProvider,
            ILifetimeScope lifetimeScope,
            IDataStoreFactory dataStoreFactory,
            IConsoleActorAccessor consoleActorAccessor,
            ICommandExecutor commandExecutor,
            ILogger<OpenModUnturnedHost> logger,
            IPluginActivator pluginActivator,
            UnturnedCommandHandler unturnedCommandHandler)
        {
            m_HostInformation = hostInformation;
            m_ServiceProvider = serviceProvider;
            m_ConsoleActorAccessor = consoleActorAccessor;
            m_CommandExecutor = commandExecutor;
            m_Logger = logger;
            m_PluginActivator = pluginActivator;
            m_UnturnedCommandHandler = unturnedCommandHandler;
            WorkingDirectory = runtime.WorkingDirectory;
            LifetimeScope = lifetimeScope;

            DataStore = dataStoreFactory.CreateDataStore(new DataStoreCreationParameters
            {
                ComponentId = OpenModComponentId,
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
            // ReSharper disable PossibleNullReferenceException
            IsComponentAlive = true;

            m_Harmony = new Harmony(OpenModComponentId);
            m_Harmony.PatchAll(GetType().Assembly);

            m_UnturnedCommandHandler.Subscribe();
            BindUnturnedEvents();

            var ioHandlers = (List<ICommandInputOutput>)typeof(CommandWindow)
                .GetField("ioHandlers", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(Dedicator.commandWindow);

            CommandLineFlag shouldManageConsole = null;
            var previousShouldManageConsoleValue = true;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                /* Fix Unturned destroying console and breaking Serilog formatting and colors */
                var windowsConsole = typeof(Provider).Assembly.GetType("SDG.Unturned.WindowsConsole");
                var shouldManageConsoleField = windowsConsole?.GetField("shouldManageConsole", BindingFlags.Static | BindingFlags.NonPublic);

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
                                         c.GetType().FullName.Equals("SDG.Unturned.ThreadedWindowsConsoleInputOutput") // type doesnt exist on Linux
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

            m_Logger.LogInformation($"OpenMod for Unturned v{m_HostInformation.HostVersion} is initializing...");

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

            m_Logger.LogInformation("OpenMod for Unturned is ready.");

            BroadcastPlugins();
            return Task.CompletedTask;
            // ReSharper restore PossibleNullReferenceException
        }

        private void BroadcastPlugins()
        {
            var pluginAdvertising = PluginAdvertising.Get();
            pluginAdvertising.PluginFrameworkName = "openmod";

            pluginAdvertising.AddPlugins(from plugin in m_PluginActivator.ActivatedPlugins
                                         where plugin.IsComponentAlive
                                         select plugin.DisplayName);
        }

        protected virtual void BindUnturnedEvents()
        {
            CommandWindow.onCommandWindowInputted += OnCommandWindowInputted;

            m_UnturnedEventsActivator = ActivatorUtilities.CreateInstance<UnturnedEventsActivator>(m_ServiceProvider);
            m_UnturnedEventsActivator.ActivateEventListeners();
        }

        protected virtual void UnbindUnturnedEvents()
        {
            // ReSharper disable DelegateSubtraction
            CommandWindow.onCommandWindowInputted -= OnCommandWindowInputted;
            // ReSharper restore DelegateSubtraction

            m_UnturnedEventsActivator.Dispose();
        }

        private void OnCommandWindowInputted(string text, ref bool shouldExecuteCommand)
        {
            shouldExecuteCommand = false;

            var actor = m_ConsoleActorAccessor.Actor;
            var args = ArgumentsParser.ParseArguments(text);
            if (args.Length == 0)
            {
                return;
            }

            AsyncHelper.Schedule("Console command execution", () => m_CommandExecutor.ExecuteAsync(actor, args, string.Empty));
        }

        public Task ShutdownAsync()
        {
            Provider.shutdown();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            RocketModIntegration.Uninstall();

            if (m_IsDisposing)
            {
                return;
            }

            Dedicator.commandWindow.removeIOHandler(m_OpenModIoHandler);

            m_IoHandlers.Reverse();
            foreach (var ioHandler in m_IoHandlers)
            {
                Dedicator.commandWindow.addIOHandler(ioHandler);
            }
            m_IoHandlers.Clear();

            IsComponentAlive = false;
            m_IsDisposing = true;
            TlsWorkaround.Uninstalll();

            m_Harmony.UnpatchAll(OpenModComponentId);
            UnbindUnturnedEvents();
        }
    }
}
