using System.Collections.Generic;

using OpenMod.API.Ioc;
using OpenMod.API.Plugins;

namespace OpenMod.Unturned.Effects
{
    /// <summary>
    /// The service for generating UI effect keys for plugins that prevents possible conflicts.
    /// </summary>
    [Service]
    public interface IUnturnedUIEffectsKeysProvider
    {
        /// <summary>
        /// Generates a unique UI key for the plugin.
        /// </summary>
        /// <param name="plugin">plugin that requests the key</param>
        /// <returns>unique UI effect key, or <see cref="UnturnedUIEffectKey.Invalid"/> if no keys are available</returns>
        UnturnedUIEffectKey BindKey(IOpenModPlugin plugin);

        /// <summary>
        /// Generates a set of unique UI keys for the plugin.
        /// </summary>
        /// <param name="plugin">plugin that requests the keys</param>
        /// <param name="amount">amount of keys generated</param>
        /// <returns>set of unique UI effect keys, or <see cref="UnturnedUIEffectKey.Invalid"/> if no keys available</returns>
        IEnumerable<UnturnedUIEffectKey> BindKeys(IOpenModPlugin plugin, int amount);

        /// <summary>
        /// Manually releases a plugin bound key to the pool of available keys.
        /// </summary>
        /// <param name="plugin">plugin that requests the key release</param>
        /// <param name="key">key to be released</param>
        /// <returns>true if the key was released, false if the key wasn't bound</returns>
        bool ReleaseKey(IOpenModPlugin plugin, UnturnedUIEffectKey key);

        /// <summary>
        /// Manually releases all plugin bound keys to the pool of available keys.
        /// </summary>
        /// <param name="plugin">plugin that requests the key release</param>
        void ReleaseAllKeys(IOpenModPlugin plugin);
    }
}