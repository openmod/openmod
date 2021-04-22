using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
using HarmonyLib;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.Core.Commands.Events;
using OpenMod.Core.Helpers;
using OpenMod.Core.Ioc;
using OpenMod.Core.Patching;
using OpenMod.Unturned.RocketMod.Commands;
using OpenMod.Unturned.RocketMod.Events;
using OpenMod.Unturned.RocketMod.Patches;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Core.RCON;
using Rocket.Unturned;
using SDG.Unturned;

namespace OpenMod.Unturned.RocketMod
{
    /// <summary>
    /// The OpenMod integration for RocketMod.
    /// </summary>
    public class RocketModIntegration : IDisposable
    {
        private const string c_HarmonyId = "com.get-openmod.unturned.module.rocketmod";
        private const BindingFlags c_BindingFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance;

        private static bool s_IsReady;

        private readonly IRocketModComponent m_RocketModComponent;
        private readonly IEventBus m_EventBus;
        private readonly ILogger<RocketModIntegration> m_Logger;
        private readonly ILogger m_RocketModLogger;

        private Harmony? m_HarmonyInstance;
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
        /// Checks if RocketMod is installed as an Unturned module.
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
        /// Checks if the given assembly is the Rocket.Unturned assembly.
        /// </summary>
        public static bool IsRocketModUnturnedAssembly(Assembly assembly)
        {
            return assembly.GetName().Name.Equals("Rocket.Unturned");
        }

        /// <summary>
        /// Checks if the given assembly is the Rocket.Core assembly.
        /// </summary>
        public static bool IsRocketModCoreAssembly(Assembly assembly)
        {
            return assembly.GetName().Name.Equals("Rocket.Core");
        }

        /// <summary>
        /// Checks if the given assembly is the Rocket.API assembly.
        /// </summary>
        public static bool IsRocketModApiAssembly(Assembly assembly)
        {
            return assembly.GetName().Name.Equals("Rocket.API");
        }

        /// <summary>
        /// Checks if the Rocket.Unturned assembly is loaded.
        /// </summary>
        public static bool IsRocketModUnturnedLoaded(out Assembly? assembly)
        {
            assembly = AppDomain.CurrentDomain.GetAssemblies().LastOrDefault(IsRocketModUnturnedAssembly);
            return assembly != null;
        }

        /// <summary>
        /// Checks if the Rocket.Core assembly is loaded.
        /// </summary>
        public static bool IsRocketModCoreLoaded(out Assembly? assembly)
        {
            assembly = AppDomain.CurrentDomain.GetAssemblies().LastOrDefault(IsRocketModCoreAssembly);
            return assembly != null;
        }

        /// <summary>
        /// Checks if the Rocket.API assembly is loaded.
        /// </summary>
        public static bool IsRocketModApiLoaded(out Assembly? assembly)
        {
            assembly = AppDomain.CurrentDomain.GetAssemblies().LastOrDefault(IsRocketModApiAssembly);
            return assembly != null;
        }

        /// <summary>
        /// Checks if RocketMod for Unturned is loaded and ready.
        /// </summary>
        public static bool IsRocketModReady()
        {
            return s_IsReady;
        }

        /// <summary>
        /// Checks if the given assembly is a RocketMod assembly.
        /// </summary>
        public static bool IsRocketModAssembly(Assembly assembly)
        {
            return IsRocketModUnturnedAssembly(assembly)
                   || IsRocketModCoreAssembly(assembly)
                   || IsRocketModApiAssembly(assembly);
        }

        /// <summary>
        /// Installs the RocketMod integration.
        /// </summary>
        /// <remarks>
        /// <b>This API is for OpenMod internal usage only and should not be called by plugins.</b>
        /// </remarks>
        [OpenModInternal]
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

            m_Logger.LogInformation("RocketMod is installed, enabling RocketMod integration");

            m_HarmonyInstance = new Harmony(c_HarmonyId);
            m_HarmonyInstance.PatchAllConditional(typeof(OpenModUnturnedHost).Assembly, "rocketmod");
            m_EventBus.Subscribe<CommandExecutedEvent>(m_RocketModComponent, (services, sender, @event) =>
            {
                var scope = services.GetRequiredService<ILifetimeScope>();
                var listener = ActivatorUtilitiesEx.CreateInstance<RocketModCommandEventListener>(scope);
                return listener.HandleEventAsync(sender, @event);
            });

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
                PatchRcon();
            }

            m_Installed = true;
        }

        private void PatchRcon()
        {
            var awakeMethod = typeof(RCONServer).GetMethod("Awake", BindingFlags.Public | BindingFlags.Instance);
            m_HarmonyInstance!.NopPatch(awakeMethod);

            var destroyMethod = typeof(RCONServer).GetMethod("OnDestroy", BindingFlags.NonPublic | BindingFlags.Instance);
            m_HarmonyInstance!.NopPatch(destroyMethod);
        }

        private void PatchPluginsLoaded()
        {
            var loadPluginsPostfixMethod = new HarmonyMethod(typeof(RocketModPluginManagerPatches).GetMethod(nameof(RocketModPluginManagerPatches.LoadPluginsPostfix), c_BindingFlags));
            m_HarmonyInstance!.Patch(typeof(RocketPluginManager).GetMethod("loadPlugins", c_BindingFlags), postfix: loadPluginsPostfixMethod);
            RocketModPluginManagerPatches.OnPostRocketPluginsLoaded += OnRocketModPluginsLoaded;
        }

        private void PatchInitialize()
        {
            var intializePostfixMethod = new HarmonyMethod(typeof(RocketModLoadPatch).GetMethod(nameof(RocketModLoadPatch.PostInitialize), c_BindingFlags));
            m_HarmonyInstance!.Patch(typeof(U).GetMethod("Initialize", c_BindingFlags), postfix: intializePostfixMethod);
            RocketModLoadPatch.OnRocketModIntialized += OnRocketModIntialized;
        }

        private void PatchLogging()
        {
            var logInternalPrefixMethod = new HarmonyMethod(typeof(RocketModLogPatches).GetMethod(nameof(RocketModLogPatches.PreLogInternal), c_BindingFlags));
            m_HarmonyInstance!.Patch(typeof(Logger).GetMethod("ProcessInternalLog", c_BindingFlags), prefix: logInternalPrefixMethod);

            var logExceptionPrefixMethod = new HarmonyMethod(typeof(RocketModLogPatches).GetMethod(nameof(RocketModLogPatches.PreLogException), c_BindingFlags));
            m_HarmonyInstance.Patch(typeof(Logger).GetMethod("LogException", c_BindingFlags), prefix: logExceptionPrefixMethod);

            RocketModLogPatches.OnRocketLog += OnRocketLog;
        }

        private void OnRocketLog(LogLevel level, string message, Exception? ex)
        {
            // ReSharper disable once TemplateIsNotCompileTimeConstantProblem
            m_RocketModLogger.Log(level, ex, message);
        }

        private void OnRocketModIntialized()
        {
            AsyncHelper.RunSync(async () => await m_EventBus.EmitAsync(m_RocketModComponent, this, new RocketModInitializedEvent()));
            m_Logger.LogInformation("RocketMod is ready");
            s_IsReady = true;
        }

        private void OnRocketModPluginsLoaded()
        {
            AsyncHelper.RunSync(async () => await m_EventBus.EmitAsync(m_RocketModComponent, this, new RocketModPluginsLoadedEvent()));
            m_Logger.LogInformation("RocketMod plugins have been loaded");
        }

        /// <summary>
        /// Disposes the RocketMod integration.
        /// </summary>
        /// <remarks>
        /// <b>This API is for OpenMod internal usage only and should not be called by plugins.</b>
        /// </remarks>
        [OpenModInternal]
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
            m_HarmonyInstance?.UnpatchAll(c_HarmonyId);
        }

        public static string GetRocketFolder()
        {
            return Path.Combine(ReadWrite.PATH, "Servers", Dedicator.serverID, "Rocket");
        }
    }
}