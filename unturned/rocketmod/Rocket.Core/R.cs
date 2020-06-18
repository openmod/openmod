using System;
using UnityEngine;
using Rocket.Core.RCON;
using Rocket.API;
using Rocket.Core.Plugins;
using Rocket.Core.Permissions;
using Rocket.Core.Utils;
using Rocket.Core.Assets;
using Rocket.API.Extensions;
using Rocket.Core.Serialization;
using Rocket.API.Collections;
using Rocket.Core.Extensions;
using Rocket.Core.Logging;
using Rocket.Core.Commands;
using System.Reflection;

namespace Rocket.Core
{
    public class R : MonoBehaviour
    {
        public delegate void RockedInitialized();
        public static event RockedInitialized OnRockedInitialized;

        public static R Instance;
        public static IRocketImplementation Implementation;

        public static XMLFileAsset<RocketSettings> Settings = null;
        public static XMLFileAsset<TranslationList> Translation = null;
        public static IRocketPermissionsProvider Permissions = null;
        public static RocketPluginManager Plugins = null;
        public static RocketCommandManager Commands = null;
        
        private static readonly TranslationList defaultTranslations = new TranslationList(){
                {"rocket_join_public","{0} connected to the server" },
                {"rocket_leave_public","{0} disconnected from the server"},
                { "command_rwho_line", "#{0}, Connection ID: {1}, Authed: {2}, Address: {3}, Time Connected: {4}, Connected For: {5}." },
                { "command_rkick_help", "Usage: rkick <ConnectionID> - Kicks a client off of RCON." },
                { "command_rkick_notfound", "Error: RCON Client with Connection ID: {0} not found!" },
                { "command_rkick_kicked", "RCON Client kicked with Connection ID: {0}, Address: {1}!" },
                { "command_rflush_help", "Usage: rflush <y> - kicks all connected RCON clients on the server." },
                { "command_rflush_total", "Closing {0} RCON connections." },
                { "command_rflush_line", "#{0}, ConnectionID: {1}, Address: {2}, closed!" },
                {"command_no_permission","You do not have permissions to execute this command."},
                {"command_cooldown","You have to wait {0} seconds before you can use this command again."}
        };
         
        private void Awake()
        {
            Instance = this;

            Implementation = (IRocketImplementation)GetComponent(typeof(IRocketImplementation));

            #if DEBUG
                gameObject.TryAddComponent<Debugger>();
            #else
                Initialize();
            #endif
        }

        internal void Initialize()
        {   
            Environment.Initialize();
            try
            {
                Implementation.OnRocketImplementationInitialized += () =>
                {
                    gameObject.TryAddComponent<TaskDispatcher>();
                    gameObject.TryAddComponent<AutomaticShutdownWatchdog>();
                    if(Settings.Instance.RCON.Enabled) gameObject.TryAddComponent<RCONServer>();
                };
                
                Settings = new XMLFileAsset<RocketSettings>(Environment.SettingsFile);
                Translation = new XMLFileAsset<TranslationList>(String.Format(Environment.TranslationFile, Settings.Instance.LanguageCode), new Type[] { typeof(TranslationList), typeof(TranslationListEntry) }, defaultTranslations);
                defaultTranslations.AddUnknownEntries(Translation);
                Permissions = gameObject.TryAddComponent<RocketPermissionsManager>();
                Plugins = gameObject.TryAddComponent<RocketPluginManager>();
                Commands = gameObject.TryAddComponent<RocketCommandManager>();

                // Load Commands from Rocket.Core.Commands.
                Commands.RegisterFromAssembly(Assembly.GetExecutingAssembly());

                if (Settings.Instance.MaxFrames < 10 && Settings.Instance.MaxFrames != -1) Settings.Instance.MaxFrames = 10;
                Application.targetFrameRate = Settings.Instance.MaxFrames;

                OnRockedInitialized.TryInvoke();
            }
            catch (Exception ex)
            {
                Logging.Logger.LogException(ex);
            }
        }

        public static string Translate(string translationKey, params object[] placeholder)
        {
            return Translation.Instance.Translate(translationKey, placeholder);
        }

        public static void Reload()
        {
            Settings.Load();
            Translation.Load();
            Permissions.Reload();
            Plugins.Reload();
            Commands.Reload();
            Implementation.Reload();
        }
    }
}