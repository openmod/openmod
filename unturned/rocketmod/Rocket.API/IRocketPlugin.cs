using Rocket.API.Collections;
using UnityEngine;

namespace Rocket.API
{
    public enum PluginState { Loaded, Unloaded, Failure, Cancelled };

    public interface IRocketPlugin<TConfiguration> : IRocketPlugin where TConfiguration : class
    {
        IAsset<TConfiguration> Configuration { get;}
    }

    public interface IRocketPlugin
    {
        string Name { get; }
        PluginState State { get; }
        TranslationList DefaultTranslations { get; }
        IAsset<TranslationList> Translations { get; }
        T TryAddComponent<T>() where T : UnityEngine.Component;
        void TryRemoveComponent<T>() where T : UnityEngine.Component;
    }
}