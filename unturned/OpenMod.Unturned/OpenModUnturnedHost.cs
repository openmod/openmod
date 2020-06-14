using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Cysharp.Threading.Tasks;
using HarmonyLib;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenMod.API;
using OpenMod.API.Commands;
using OpenMod.API.Ioc;
using OpenMod.API.Persistence;
using OpenMod.Core.Helpers;
using OpenMod.UnityEngine;
using OpenMod.Unturned.Console;
using OpenMod.Unturned.Helpers;
using OpenMod.Unturned.Logging;
using OpenMod.Unturned.Players;
using SDG.Unturned;
using UnityEngine;
using UnityEngine.Experimental.LowLevel;
using Priority = OpenMod.API.Prioritization.Priority;

namespace OpenMod.Unturned
{
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    public class OpenModUnturnedHost : IOpenModHost, IDisposable
    {
        private readonly ILoggerFactory m_LoggerFactory;
        private readonly IConsoleActorAccessor m_ConsoleActorAccessor;
        private readonly ICommandExecutor m_CommandExecutor;
        public string DisplayName { get; } = Provider.APP_NAME;
        public string Version { get; } = Provider.APP_VERSION;
        public string OpenModComponentId { get; } = "OpenMod.Unturned";
        public string WorkingDirectory { get; }
        public bool IsComponentAlive { get; private set; }
        public ILifetimeScope LifetimeScope { get; }
        public IDataStore DataStore { get; }

        private const string HarmonyInstanceId = "com.get-openmod.unturned";
        private readonly Harmony m_Harmony;
        private bool m_IsDisposing;
        private bool m_IsProviderShutdown;

        public OpenModUnturnedHost(
            IRuntime runtime,
            ILifetimeScope lifetimeScope,
            IDataStoreFactory dataStoreFactory,
            ILoggerFactory loggerFactory,
            IConsoleActorAccessor consoleActorAccessor,
            ICommandExecutor commandExecutor)
        {
            m_LoggerFactory = loggerFactory;
            m_ConsoleActorAccessor = consoleActorAccessor;
            m_CommandExecutor = commandExecutor;
            m_Harmony = new Harmony(HarmonyInstanceId);
            WorkingDirectory = runtime.WorkingDirectory;
            LifetimeScope = lifetimeScope;
            DataStore = dataStoreFactory.CreateDataStore("openmod.unturned", WorkingDirectory);

            Provider.onServerShutdown += OnServerShutdown;
        }

        public Task InitAsync()
        {
            IsComponentAlive = true;

            // the following code *must* run on main thread
            // we can not use UniTask because it is not set up yet
            // we can also not wait for it to complete or it will cause a deadlock (main thread is already waiting for this method to complete)
            UnityMainThreadDispatcher.Instance.EnqueueAsync(() =>
            {
                BindUnturnedEvents();

                //if (PlatformHelper.IsLinux)
                //{
                //    Dedicator.commandWindow.setIOHandler(new SerilogConsoleInputOutput(m_LoggerFactory));
                //}
                //else
                //{
                //    Dedicator.commandWindow.setIOHandler(new SerilogWindowsConsoleInputOutput(m_LoggerFactory));
                //}
                
                m_Harmony.PatchAll(GetType().Assembly);
                TlsWorkaround.Install();

                var unitySynchronizationContetextField = typeof(PlayerLoopHelper).GetField("unitySynchronizationContetext", BindingFlags.Static | BindingFlags.NonPublic);
                unitySynchronizationContetextField.SetValue(null, SynchronizationContext.Current);
                var mainThreadIdField = typeof(PlayerLoopHelper).GetField("mainThreadId", BindingFlags.Static | BindingFlags.NonPublic);
                mainThreadIdField.SetValue(null, Thread.CurrentThread.ManagedThreadId);

                var playerLoop = PlayerLoop.GetDefaultPlayerLoop();
                PlayerLoopHelper.Initialize(ref playerLoop);
            });

            return Task.CompletedTask;
        }

        protected virtual void BindUnturnedEvents()
        {
            CommandWindow.onCommandWindowInputted += (string text, ref bool shouldExecuteCommand) =>
            {
                var actor = m_ConsoleActorAccessor.Actor;
                AsyncHelper.Schedule("Console command execution", () => m_CommandExecutor.ExecuteAsync(actor, text.Split(' '), string.Empty));
                shouldExecuteCommand = false;
            };

            ChatManager.onCheckPermissions += (SteamPlayer player, string text, ref bool shouldExecuteCommand, ref bool shouldList) =>
            {
                if (!text.StartsWith("/"))
                {
                    return;
                }

                shouldExecuteCommand = false;
                shouldList = false;

                var actor = new PlayerCommandActor(player.player);
                AsyncHelper.Schedule("Player command execution", () => m_CommandExecutor.ExecuteAsync(actor, text.Split(' '), string.Empty));
            };
        }

        private void OnServerShutdown()
        {
            if (!m_IsDisposing)
            {
                m_IsProviderShutdown = true;
            }
        }

        public void Dispose()
        {
            if (m_IsDisposing)
            {
                return;
            }

            IsComponentAlive = false;
            m_IsDisposing = true;

            m_Harmony.UnpatchAll(HarmonyInstanceId);

            if (!m_IsProviderShutdown)
            {
                Provider.shutdown();
            }
        }
    }
}
