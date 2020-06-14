using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Cysharp.Threading.Tasks;
using HarmonyLib;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API;
using OpenMod.API.Ioc;
using OpenMod.API.Persistence;
using OpenMod.Core.Helpers;
using OpenMod.UnityEngine;
using OpenMod.Unturned.Helpers;
using OpenMod.Unturned.Logging;
using SDG.Unturned;
using UnityEngine;
using UnityEngine.Experimental.LowLevel;
using Priority = OpenMod.API.Prioritization.Priority;

namespace OpenMod.Unturned
{
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    public class OpenModUnturnedHost : IOpenModHost, IDisposable
    {
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
        
        public OpenModUnturnedHost(IRuntime runtime, ILifetimeScope lifetimeScope, IDataStoreFactory dataStoreFactory)
        {
            m_Harmony = new Harmony(HarmonyInstanceId);
            WorkingDirectory = runtime.WorkingDirectory;
            LifetimeScope = lifetimeScope;
            DataStore = dataStoreFactory.CreateDataStore("openmod.unturned", WorkingDirectory);

            Provider.onServerShutdown += OnServerShutdown;
        }

        public Task InitAsync()
        {
            IsComponentAlive = true;
            return UnityMainThreadDispatcher.Instance.EnqueueAsync(() =>
            {
                if (PlatformHelper.IsLinux)
                {
                    Dedicator.commandWindow.setIOHandler(new SerilogConsoleInputOutput());
                }
                else
                {
                    Dedicator.commandWindow.setIOHandler(new SerilogWindowsConsoleInputOutput());
                }

                var logCallbackField = typeof(Application).GetField("s_LogCallbackHandler", BindingFlags.Static | BindingFlags.Instance);
                logCallbackField.SetValue(null, (Application.LogCallback)DummyLogCallback);

                m_Harmony.PatchAll(GetType().Assembly);
                TlsWorkaround.Install();

                var unitySynchronizationContetextField = typeof(PlayerLoopHelper).GetField("unitySynchronizationContetext", BindingFlags.Static | BindingFlags.NonPublic);
                unitySynchronizationContetextField.SetValue(null, SynchronizationContext.Current);
                var mainThreadIdField = typeof(PlayerLoopHelper).GetField("mainThreadId", BindingFlags.Static | BindingFlags.NonPublic);
                mainThreadIdField.SetValue(null, Thread.CurrentThread.ManagedThreadId);

                var playerLoop = PlayerLoop.GetDefaultPlayerLoop();
                PlayerLoopHelper.Initialize(ref playerLoop);
            });
        }

        private void DummyLogCallback(string condition, string stacktrace, LogType type)
        {
            // do nothing
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
