using System.Collections.Generic;

using OpenMod.API;
using OpenMod.API.Ioc;

namespace OpenMod.Unturned.Effects
{
    /// <summary>
    /// The service for generating UI effect keys for components that prevents possible conflicts.
    /// </summary>
    [Service]
    public interface IUnturnedUIEffectsKeysProvider
    {
        /// <summary>
        /// Generates a unique UI key for the component.
        /// </summary>
        /// <param name="component">component that requests the key</param>
        /// <returns>unique UI effect key, or <see cref="UnturnedUIEffectKey.Invalid"/> if no keys are available</returns>
        UnturnedUIEffectKey BindKey(IOpenModComponent component);

        /// <summary>
        /// Generates a set of unique UI keys for the component.
        /// </summary>
        /// <param name="component">component that requests the keys</param>
        /// <param name="amount">amount of keys generated</param>
        /// <returns>set of unique UI effect keys, or <see cref="UnturnedUIEffectKey.Invalid"/> if no keys available</returns>
        IEnumerable<UnturnedUIEffectKey> BindKeys(IOpenModComponent component, int amount);

        /// <summary>
        /// Manually releases a component bound key to the pool of available keys.
        /// </summary>
        /// <param name="component">component that requests the key release</param>
        /// <param name="key">key to be released</param>
        /// <returns>true if the key was released, false if the key wasn't bound</returns>
        bool ReleaseKey(IOpenModComponent component, UnturnedUIEffectKey key);

        /// <summary>
        /// Manually releases all component bound keys to the pool of available keys.
        /// </summary>
        /// <param name="component">component that requests the key release</param>
        void ReleaseAllKeys(IOpenModComponent component);
    }
}