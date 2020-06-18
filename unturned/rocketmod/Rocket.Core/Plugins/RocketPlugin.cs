using Rocket.API;
using Rocket.API.Collections;
using Rocket.API.Extensions;
using Rocket.Core.Assets;
using Rocket.Core.Extensions;
using Rocket.Core.Logging;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Rocket.Core.Plugins
{
    public class RocketPlugin<RocketPluginConfiguration> : RocketPlugin, IRocketPlugin<RocketPluginConfiguration> where RocketPluginConfiguration : class, IRocketPluginConfiguration
    {
        private IAsset<RocketPluginConfiguration> configuration;
        public IAsset<RocketPluginConfiguration> Configuration { get { return configuration; } }

        public RocketPlugin() : base()
        {
            string configurationFile = Path.Combine(Directory, string.Format(Core.Environment.PluginConfigurationFileTemplate, Name));

            string url = "";
            
            if (Core.R.Settings.Instance.WebConfigurations.Enabled) {
                url = string.Format(Environment.WebConfigurationTemplate, Core.R.Settings.Instance.WebConfigurations.Url, Name, R.Implementation.InstanceId);
            }else if (File.Exists(configurationFile)) { 
                url = File.ReadAllLines(configurationFile).First().Trim();
            }

            Uri uri;
            if (Uri.TryCreate(url,UriKind.Absolute,out uri))
            {
                configuration = new WebXMLFileAsset<RocketPluginConfiguration>(uri, null, (IAsset<RocketPluginConfiguration> asset) => { base.LoadPlugin(); });
            }
            else
            {
                configuration = new XMLFileAsset<RocketPluginConfiguration>(configurationFile);
            }
        }

        public override void LoadPlugin()
        {
            configuration.Load((IAsset<RocketPluginConfiguration> asset)=> { base.LoadPlugin(); });
        }
    }

    public class RocketPlugin : MonoBehaviour, IRocketPlugin
    {
        public delegate void PluginUnloading(IRocketPlugin plugin);
        public static event PluginUnloading OnPluginUnloading;

        public delegate void PluginLoading(IRocketPlugin plugin, ref bool cancelLoading);
        public static event PluginLoading OnPluginLoading;

        private XMLFileAsset<TranslationList> translations;
        public IAsset<TranslationList> Translations { get { return translations; } }

        private PluginState state = PluginState.Unloaded;
        public PluginState State
        {
            get
            {
                return state;
            }
        }

        public Assembly Assembly { get; private set; }
        public string Directory { get; private set; }
        public string Name { get; private set; }

        public virtual TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList();
            }
        }

        public RocketPlugin()
        {
            Assembly = GetType().Assembly;
            Name = Assembly.GetName().Name;
            Directory = Path.Combine(Core.Environment.PluginsDirectory, Name); // String.Format(Core.Environment.PluginDirectory, Name);
            if (!System.IO.Directory.Exists(Directory))
                System.IO.Directory.CreateDirectory(Directory);

            if (DefaultTranslations != null | DefaultTranslations.Count() != 0)
            {
                translations = new XMLFileAsset<TranslationList>(Path.Combine(Directory,String.Format(Environment.PluginTranslationFileTemplate, Name, R.Settings.Instance.LanguageCode)), new Type[] { typeof(TranslationList), typeof(TranslationListEntry) }, DefaultTranslations);
                DefaultTranslations.AddUnknownEntries(translations);
            }
        }

        public static bool IsDependencyLoaded(string plugin)
        {
            return Rocket.Core.R.Plugins.GetPlugin(plugin) != null;
        }

        public delegate void ExecuteDependencyCodeDelegate(IRocketPlugin plugin); 
        public static void ExecuteDependencyCode(string plugin,ExecuteDependencyCodeDelegate a)
        {
            IRocketPlugin p = Rocket.Core.R.Plugins.GetPlugin(plugin);
            if (p != null) 
                a(p);
        }

        public string Translate(string translationKey, params object[] placeholder)
        {
            return Translations.Instance.Translate(translationKey,placeholder);
        }

        public void ReloadPlugin()
        {
            UnloadPlugin();
            LoadPlugin();
        }

        public virtual void LoadPlugin()
        {
            Logging.Logger.Log("\n[loading] " + Name, ConsoleColor.Cyan);
            translations.Load();
            R.Commands.RegisterFromAssembly(Assembly);

            try
            {
                Load();
            }
            catch (Exception ex)
            {
                Logging.Logger.LogError("Failed to load " + Name + ", unloading now... :" + ex.ToString());
                try
                {
                    UnloadPlugin(PluginState.Failure);
                    return;
                }
                catch (Exception ex1)
                {
                    Logging.Logger.LogError("Failed to unload " + Name + ":" + ex1.ToString());
                }
            }
            
            bool cancelLoading = false;
            if (OnPluginLoading != null)
            {
                foreach (var handler in OnPluginLoading.GetInvocationList().Cast<PluginLoading>())
                {
                    try
                    {
                        handler(this, ref cancelLoading);
                    }
                    catch (Exception ex)
                    {
                        Logging.Logger.LogException(ex);
                    }
                    if (cancelLoading) {
                        try
                        {
                            UnloadPlugin(PluginState.Cancelled);
                            return;
                        }
                        catch (Exception ex1)
                        {
                            Logging.Logger.LogError("Failed to unload " + Name + ":" + ex1.ToString());
                        }
                    }
                }
            }
            state = PluginState.Loaded;
        }

        public virtual void UnloadPlugin(PluginState state = PluginState.Unloaded)
        {
            Logging.Logger.Log("\n[unloading] " + Name, ConsoleColor.Cyan);
            OnPluginUnloading.TryInvoke(this);
            R.Commands.DeregisterFromAssembly(Assembly);
            Unload();
            this.state = state;
        }

        private void OnEnable()
        {
                LoadPlugin();
        }

        private void OnDisable()
        {
            UnloadPlugin();
        }

        protected virtual void Load()
        {

        }


        protected virtual void Unload()
        {

        }

        public T TryAddComponent<T>() where T : Component
        {
            return gameObject.TryAddComponent<T>();
        }

        public void TryRemoveComponent<T>() where T : Component
        {
            gameObject.TryRemoveComponent<T>();
        }
    }
}
