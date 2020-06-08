using System;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using HarmonyLib;
using Microsoft.Extensions.Logging;
using OpenMod.API;
using OpenMod.API.Ioc;
using OpenMod.Core.Helpers;
using OpenMod.UnityEngine;
using OpenMod.Unturned.Helpers;
using OpenMod.Unturned.Logging;
using SDG.Unturned;
using Serilog;
using UnityEngine;
using UnityEngine.Experimental.LowLevel;

namespace OpenMod.Unturned
{
    [ServiceImplementation]
    public class OpenModUnturnedHost : IOpenModHost, IDisposable
    {
        private readonly Harmony m_Harmony;
        private const string HarmonyInstanceId = "com.get-openmod.unturned";

        public OpenModUnturnedHost(IRuntime runtime)
        {
            m_Harmony = new Harmony(HarmonyInstanceId);
            WorkingDirectory = runtime.WorkingDirectory;
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


        public string DisplayName { get; } = Provider.APP_NAME;

        public string Version { get; } = Provider.APP_VERSION;

        public void Dispose()
        {
            IsComponentAlive = false;
            m_Harmony.UnpatchAll(HarmonyInstanceId);
        }

        public string OpenModComponentId { get; } = "OpenMod.Unturned";
        public string WorkingDirectory { get; }
        public bool IsComponentAlive { get; private set; }
    }
}
