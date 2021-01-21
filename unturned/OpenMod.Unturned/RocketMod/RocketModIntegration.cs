using System;
using System.IO;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Microsoft.Extensions.Logging;
using OpenMod.API.Eventing;
using OpenMod.Core.Helpers;
using OpenMod.Core.Patching;
using OpenMod.Unturned.RocketMod.Events;
using OpenMod.Unturned.RocketMod.Patches;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using SDG.Unturned;

namespace OpenMod.Unturned.RocketMod
{
    public class RocketModIntegration : IDisposable
    {
        private const string c_HarmonyId = "com.get-openmod.unturned.module.rocketmod";
        private const BindingFlags c_BindingFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance;

        private static bool s_IsReady;

        private readonly IRocketModComponent m_RocketModComponent;
        private readonly IEventBus m_EventBus;
        private readonly ILogger<RocketModIntegration> m_Logger;
        private readonly ILogger m_RocketModLogger;

        private Harmony m_HarmonyInstance;
        private bool m_Installed;

        public RocketModIntegration(
            IRocketModComponent rocketModComponent,
            IEventBus eventBus,
            ILogger<RocketModIntegration> logger,
            ILoggerFactory loggerFactory)
        {
            m_RocketModComponent = rocketModComponent;
            m_EventBus = eventBus;
            m_Logger = logger;
            m_RocketModLogger = loggerFactory.CreateLogger("RocketMod");
        }

        /// <summary>
        /// Returns true if RocketMod is installed as an Unturned module.
        /// </summary>
        /// <returns></returns>
        public static bool IsRocketModInstalled()
        {
            var modulesDirectory = Path.Combine(ReadWrite.PATH, "Modules");
            const string rocketModuleFile = "Rocket.Unturned.module";

            return Directory.GetFiles(modulesDirectory, rocketModuleFile, SearchOption.AllDirectories)
                .Any();
        }

        /// <summary>
        /// Returns true if the given assembly is the Rocket.Unturned assembly.
        /// </summary>
        public static bool IsRocketModUnturnedAssembly(Assembly assembly)
        {
            return assembly.GetName().Name.Equals("Rocket.Unturned");
        }

        /// <summary>
        /// Returns true if the given assembly is the Rocket.Core assembly.
        /// </summary>
        public static bool IsRocketModCoreAssembly(Assembly assembly)
        {
            return assembly.GetName().Name.Equals("Rocket.Core");
        }

        /// <summary>
        /// Returns true if the given assembly is the Rocket.API assembly.
        /// </summary>
        public static bool IsRocketModApiAssembly(Assembly assembly)
        {
            return assembly.GetName().Name.Equals("Rocket.API");
        }

        /// <summary>
        /// Returns true if the Rocket.Unturned assembly is loaded.
        /// </summary>
        public static bool IsRocketModUnturnedLoaded(out Assembly assembly)
        {
            assembly = AppDomain.CurrentDomain.GetAssemblies().LastOrDefault(IsRocketModUnturnedAssembly);
            return assembly != null;
        }

        /// <summary>
        /// Returns true if the Rocket.Core assembly is loaded.
        /// </summary>
        public static bool IsRocketModCoreLoaded(out Assembly assembly)
        {
            assembly = AppDomain.CurrentDomain.GetAssemblies().LastOrDefault(IsRocketModCoreAssembly);
            return assembly != null;
        }

        /// <summary>
        /// Returns true if the Rocket.API assembly is loaded.
        /// </summary>
        public static bool IsRocketModApiLoaded(out Assembly assembly)
        {
            assembly = AppDomain.CurrentDomain.GetAssemblies().LastOrDefault(IsRocketModApiAssembly);
            return assembly != null;
        }

        /// <summary>
        /// Returns true if RocketMod for Unturned is loaded and ready.
        /// </summary>
        public static bool IsRocketModReady()
        {
            return s_IsReady;
        }

        /// <summary>
        /// Returns true if the given assembly is a RocketMod assembly.
        /// </summary>
        public static bool IsRocketModAssembly(Assembly assembly)
        {
            return IsRocketModUnturnedAssembly(assembly)
                   || IsRocketModCoreAssembly(assembly)
                   || IsRocketModApiAssembly(assembly);
        }


        public void Install()
        {
            if (!IsRocketModInstalled())
            {
                return;
            }

            if (!IsRocketModUnturnedLoaded(out _))
            {
                return;
            }

            if (m_Installed)
            {
                return;
            }

            m_Logger.LogInformation("RocketMod is installed, enabling RocketMod integration.");

            m_HarmonyInstance = new Harmony(c_HarmonyId);
            m_HarmonyInstance.PatchAllConditional(typeof(OpenModUnturnedHost).Assembly, "rocketmod");

            if (U.Settings != null)
            {
                // Rocketmod already initialized
                OnRocketModIntialized();
                OnRocketModPluginsLoaded();
            }
            else
            {
                PatchInitialize();
                PatchLogging();
                PatchPluginsLoaded();
            }

            m_Installed = true;
        }

        private void PatchPluginsLoaded()
        {
            var loadPluginsPostfixMethod = new HarmonyMethod(typeof(RocketModPluginManagerPatches).GetMethod(nameof(RocketModPluginManagerPatches.LoadPluginsPostfix), c_BindingFlags));
            m_HarmonyInstance.Patch(typeof(RocketPluginManager).GetMethod("loadPlugins", c_BindingFlags), postfix: loadPluginsPostfixMethod);
            RocketModPluginManagerPatches.OnPostRocketPluginsLoaded += OnRocketModPluginsLoaded;
        }

        private void PatchInitialize()
        {
            var intializePostfixMethod = new HarmonyMethod(typeof(RocketModLoadPatch).GetMethod(nameof(RocketModLoadPatch.PostInitialize), c_BindingFlags));
            m_HarmonyInstance.Patch(typeof(U).GetMethod("Initialize", c_BindingFlags), postfix: intializePostfixMethod);
            RocketModLoadPatch.OnRocketModIntialized += OnRocketModIntialized;
        }

        private void PatchLogging()
        {
            var logInternalPrefixMethod = new HarmonyMethod(typeof(RocketModLogPatches).GetMethod(nameof(RocketModLogPatches.PreLogInternal), c_BindingFlags));
            m_HarmonyInstance.Patch(typeof(Logger).GetMethod("ProcessInternalLog", c_BindingFlags), prefix: logInternalPrefixMethod);

            var logExceptionPrefixMethod = new HarmonyMethod(typeof(RocketModLogPatches).GetMethod(nameof(RocketModLogPatches.PreLogException), c_BindingFlags));
            m_HarmonyInstance.Patch(typeof(Logger).GetMethod("LogException", c_BindingFlags), prefix: logExceptionPrefixMethod);

            var externalLogPrefixMethod = new HarmonyMethod(typeof(RocketModLogPatches).GetMethod(nameof(RocketModLogPatches.PreExternalLog), c_BindingFlags));
            m_HarmonyInstance.Patch(typeof(Logger).GetMethod("ExternalLog", c_BindingFlags), prefix: externalLogPrefixMethod);

            RocketModLogPatches.OnRocketLog += OnRocketLog;
        }

        private void OnRocketLog(LogLevel level, string message, Exception ex)
        {
            m_RocketModLogger.Log(level, ex, message);
        }

        private void OnRocketModIntialized()
        {
            AsyncHelper.RunSync(async () => await m_EventBus.EmitAsync(m_RocketModComponent, this, new RocketModReadyEvent()));
            m_Logger.LogInformation("RocketMod is ready.");
            s_IsReady = true;
        }
        private void OnRocketModPluginsLoaded()
        {
            AsyncHelper.RunSync(async () => await m_EventBus.EmitAsync(m_RocketModComponent, this, new RocketModPluginsLoadedEvent()));
            m_Logger.LogInformation("RocketMod plugins have been loaded.");
        }

        public void Dispose()
        {
            if (!m_Installed)
            {
                return;
            }

            RocketModLoadPatch.OnRocketModIntialized -= OnRocketModIntialized;
            RocketModLogPatches.OnRocketLog -= OnRocketLog;

            if (!IsRocketModUnturnedLoaded(out _)) // in case Nelson and Unity update to .NET 5 and somehow Rocket can be unloaded
            {
                return;
            }

            s_IsReady = false;
            m_Installed = false;
            m_HarmonyInstance.UnpatchAll(c_HarmonyId);
        }

        public static string GetRocketFolder()
        {
            return Path.Combine(ReadWrite.PATH, "Servers", Dedicator.serverID, "Rocket");
        }
    }
}