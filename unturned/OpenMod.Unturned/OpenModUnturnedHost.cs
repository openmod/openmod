using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Cysharp.Threading.Tasks;
using HarmonyLib;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenMod.API;
using OpenMod.API.Commands;
using OpenMod.API.Ioc;
using OpenMod.API.Persistence;
using OpenMod.Core.Console;
using OpenMod.Core.Helpers;
using OpenMod.UnityEngine;
using OpenMod.Unturned.Commands;
using OpenMod.Unturned.Helpers;
using OpenMod.Unturned.Logging;
using OpenMod.Unturned.Users;
using SDG.Unturned;
using Semver;
using UnityEngine;
using UnityEngine.Experimental.LowLevel;
using Priority = OpenMod.API.Prioritization.Priority;

namespace OpenMod.Unturned
{
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    public class OpenModUnturnedHost : IOpenModHost, IDisposable
    {
        private readonly IRuntime m_Runtime;
        private readonly ILoggerFactory m_LoggerFactory;
        private readonly IConsoleActorAccessor m_ConsoleActorAccessor;
        private readonly ICommandExecutor m_CommandExecutor;
        private readonly ILogger<OpenModUnturnedHost> m_Logger;
        private readonly UnturnedCommandHandler m_UnturnedCommandHandler;
        public string HostDisplayName { get; } = Provider.APP_NAME;
        public string HostVersion { get; } = Provider.APP_VERSION;
        public SemVersion Version { get; }
        public string Name { get; } = "OpenMod for Unturned";
        public string OpenModComponentId { get; } = "OpenMod.Unturned";
        public string WorkingDirectory { get; }
        public bool IsComponentAlive { get; private set; }
        public ILifetimeScope LifetimeScope { get; }
        public IDataStore DataStore { get; }

        private const string c_HarmonyInstanceId = "com.get-openmod.unturned";
        private readonly Harmony m_Harmony;
        private bool m_IsDisposing;

        // ReSharper disable once SuggestBaseTypeForParameter /* we don't want this because of DI */
        public OpenModUnturnedHost(
            IRuntime runtime,
            ILifetimeScope lifetimeScope,
            IDataStoreFactory dataStoreFactory,
            ILoggerFactory loggerFactory,
            IConsoleActorAccessor consoleActorAccessor,
            ICommandExecutor commandExecutor,
            ILogger<OpenModUnturnedHost> logger,
            UnturnedCommandHandler unturnedCommandHandler)
        {
            m_Runtime = runtime;
            m_LoggerFactory = loggerFactory;
            m_ConsoleActorAccessor = consoleActorAccessor;
            m_CommandExecutor = commandExecutor;
            m_Logger = logger;
            m_UnturnedCommandHandler = unturnedCommandHandler;
            m_Harmony = new Harmony(c_HarmonyInstanceId);
            WorkingDirectory = runtime.WorkingDirectory;
            LifetimeScope = lifetimeScope;
            DataStore = dataStoreFactory.CreateDataStore("openmod.unturned", WorkingDirectory);
            Version = VersionHelper.ParseAssemblyVersion(GetType().Assembly);
        }

        public Task InitAsync()
        {
            // ReSharper disable PossibleNullReferenceException
            IsComponentAlive = true;

            m_UnturnedCommandHandler.Subscribe();
            BindUnturnedEvents();

            var ioHandlers = (List<ICommandInputOutput>) typeof(CommandWindow)
                .GetField("ioHandlers", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(Dedicator.commandWindow);

            /* Fix Unturned destroying console and breaking Serilog formatting and colors */
            var shouldManageConsoleField = typeof(WindowsConsole).GetField("shouldManageConsole", BindingFlags.Static | BindingFlags.NonPublic);
            var shouldManageConsole = (CommandLineFlag)shouldManageConsoleField.GetValue(null);
            shouldManageConsole.value = false;

            // unturned built-in io handlers
            var defaultIoHandlers = ioHandlers.Where(c => c.GetType() == typeof(ThreadedWindowsConsoleInputOutput)
                                      || c.GetType() == typeof(WindowsConsoleInputOutput)
                                      || c.GetType() == typeof(ThreadedConsoleInputOutput)
                                      || c.GetType() == typeof(ConsoleInputOutput)).ToList();

            foreach (var ioHandler in defaultIoHandlers)
            {
                Dedicator.commandWindow.removeIOHandler(ioHandler);
            }

            if (PlatformHelper.IsLinux)
            {
                Dedicator.commandWindow.addIOHandler(new SerilogConsoleInputOutput(m_LoggerFactory));
            }
            else
            {
                Dedicator.commandWindow.addIOHandler(new SerilogWindowsConsoleInputOutput(m_LoggerFactory));
            }

            m_Logger.LogInformation($"OpenMod for Unturned v{Version} is initializing...");

            m_Harmony.PatchAll(typeof(OpenModUnturnedHost).Assembly);
            TlsWorkaround.Install();

            var unitySynchronizationContetextField = typeof(PlayerLoopHelper).GetField("unitySynchronizationContetext", BindingFlags.Static | BindingFlags.NonPublic);
            unitySynchronizationContetextField.SetValue(null, SynchronizationContext.Current);

            var mainThreadIdField = typeof(PlayerLoopHelper).GetField("mainThreadId", BindingFlags.Static | BindingFlags.NonPublic);
            mainThreadIdField.SetValue(null, Thread.CurrentThread.ManagedThreadId);

            var playerLoop = PlayerLoop.GetDefaultPlayerLoop();
            PlayerLoopHelper.Initialize(ref playerLoop);
            
            m_Logger.LogInformation("OpenMod for Unturned is ready.");
            return Task.CompletedTask;
            // ReSharper restore PossibleNullReferenceException
        }

        protected virtual void BindUnturnedEvents()
        {
            Provider.onCommenceShutdown += OnServerShutdown;
            CommandWindow.onCommandWindowInputted += OnCommandWindowInputted;
        }

        protected virtual void UnbindUnturnedEvents()
        {
            // ReSharper disable DelegateSubtraction
            Provider.onCommenceShutdown -= OnServerShutdown;
            CommandWindow.onCommandWindowInputted -= OnCommandWindowInputted;
            // ReSharper restore DelegateSubtraction
        }


        private void OnCommandWindowInputted(string text, ref bool shouldExecuteCommand)
        {
            if (!shouldExecuteCommand)
                return;
            text = text.Trim();

            var actor = m_ConsoleActorAccessor.Actor;
            AsyncHelper.Schedule("Console command execution", () => m_CommandExecutor.ExecuteAsync(actor, text.Split(' '), string.Empty));
            shouldExecuteCommand = false;
        }

        private void OnServerShutdown()
        {
            AsyncHelper.RunSync(() => m_Runtime.ShutdownAsync());
        }

        public void Dispose()
        {
            if (m_IsDisposing)
            {
                return;
            }

            IsComponentAlive = false;
            m_IsDisposing = true;

            m_Harmony.UnpatchAll(c_HarmonyInstanceId);
            UnbindUnturnedEvents();
        }
    }
}
