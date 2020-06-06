using System;
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
using UnityEngine.Experimental.LowLevel;

namespace OpenMod.Unturned
{
    [ServiceImplementation]
    public class OpenModUnturnedHost : IOpenModHost, IDisposable
    {
        private readonly ILogger<OpenModUnturnedHost> m_Logger;
        private readonly Harmony m_Harmony;
        private const string HarmonyInstanceId = "com.get-openmod.unturned";
        
        public OpenModUnturnedHost(ILogger<OpenModUnturnedHost> logger)
        {
            m_Harmony = new Harmony(HarmonyInstanceId);
            m_Logger = logger;
        }

        public Task InitAsync()
        {
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

                m_Harmony.PatchAll(GetType().Assembly);
                TlsWorkaround.Install();

                Log.Information("Setting unitySynchronizationContetext and mainThreadId");
                var unitySynchronizationContetextField = typeof(PlayerLoopHelper).GetField("unitySynchronizationContetext", BindingFlags.Static | BindingFlags.NonPublic);
                unitySynchronizationContetextField.SetValue(null, SynchronizationContext.Current);
                var mainThreadIdField = typeof(PlayerLoopHelper).GetField("mainThreadId", BindingFlags.Static | BindingFlags.NonPublic);
                mainThreadIdField.SetValue(null, Thread.CurrentThread.ManagedThreadId);

                Log.Information("Initializing player loop");
                var playerLoop = PlayerLoop.GetDefaultPlayerLoop();
                PlayerLoopHelper.Initialize(ref playerLoop);
            });
        }

        public string Name { get; } = Provider.APP_NAME;

        public string Version { get; } = Provider.APP_VERSION;

        public void Dispose()
        {
            m_Harmony.UnpatchAll(HarmonyInstanceId);
        }
    }
}
