using Autofac.Core.Lifetime;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MoreLinq;
using OpenMod.API;
using OpenMod.API.Ioc;
using OpenMod.API.Prioritization;
using System.Collections.Generic;
using System.Linq;

namespace OpenMod.Unturned.Effects
{
    [OpenModInternal]
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    public class UnturnedUIEffectsKeysProvider : IUnturnedUIEffectsKeysProvider
    {
        private readonly ILogger<UnturnedUIEffectsKeysProvider> m_Logger;

        private short m_CurrentMaxKey = short.MinValue;

        /// <summary>
        /// Dictionary that holds effect key bindings
        /// </summary>
        private readonly Dictionary<IOpenModComponent, List<UnturnedUIEffectKey>> m_Bindings = new();

        private readonly List<UnturnedUIEffectKey> m_ReleasedKeys = new();

        public UnturnedUIEffectsKeysProvider(ILogger<UnturnedUIEffectsKeysProvider> logger)
        {
            m_Logger = logger;
        }

        public UnturnedUIEffectKey BindKey(IOpenModComponent component)
        {
            // component must be alive to get a key
            if (!component.IsComponentAlive)
            {
                m_Logger.LogDebug("{ComponentId} tried to bind a UI effect key but the component is not alive", component.OpenModComponentId);
                return UnturnedUIEffectKey.Invalid;
            }

            // attempt to generate the key
            UnturnedUIEffectKey? result = GetNewKey();

            if (result != null)
            { // if we got a key, bind it
                List<UnturnedUIEffectKey> bindings = GetOrCreateBindingsFor(component);
                bindings.Add(result.Value);

                return result.Value;
            }

            // all keys are taken, log biggest offender
            LogBiggestOffender(component.OpenModComponentId);

            return UnturnedUIEffectKey.Invalid;
        }

        public IEnumerable<UnturnedUIEffectKey> BindKeys(IOpenModComponent component, int amount)
        {
            // component must be alive to get keys
            if (!component.IsComponentAlive)
            {
                m_Logger.LogDebug(
                    "{ComponentId} tried to bind a UI effect keys but the component is not alive",
                    component.OpenModComponentId);
                return Enumerable.Repeat(UnturnedUIEffectKey.Invalid, amount);
            }

            // attempt to generate keys
            List<UnturnedUIEffectKey> result = new(amount);
            bool logOffender = false;
            for (int i = 0; i < amount; i++)
            {
                UnturnedUIEffectKey? key = GetNewKey();

                if (key == null)
                {
                    logOffender = true;
                }

                result.Add(key ?? UnturnedUIEffectKey.Invalid);
            }

            // bind generated keys (except invalid ones)
            List<UnturnedUIEffectKey> bindings = GetOrCreateBindingsFor(component);
            bindings.AddRange(result.Where(x => x != UnturnedUIEffectKey.Invalid));

            // all keys are taken, log biggest offender
            if (logOffender)
            {
                LogBiggestOffender(component.OpenModComponentId);
            }

            return result;
        }

        public bool ReleaseKey(IOpenModComponent component, UnturnedUIEffectKey key)
        {
            if (!m_Bindings.TryGetValue(component, out List<UnturnedUIEffectKey> bindings))
            { // skip if there's no bindings for the component
                return false;
            }

            // try to remove the key from bindings list
            bool result = bindings.Remove(key);
            if (result)
            { // add the key to list of released keys for reuse
                m_ReleasedKeys.Add(key);
            }

            if (!bindings.Any())
            { // cleanup all component related stuff when there's no bindings left
                CleanupComponent(component);
            }

            return result;
        }

        public void ReleaseAllKeys(IOpenModComponent component)
        {
            if (m_Bindings.TryGetValue(component, out List<UnturnedUIEffectKey> bindings))
            {
                // add released keys to list for reuse
                m_ReleasedKeys.AddRange(bindings);
                // cleanup all component related stuff
                CleanupComponent(component);
            }
        }

        private UnturnedUIEffectKey? GetNewKey()
        {
            // grab keys by incrementing the value while we still have more available (first 65535 keys)
            if (m_CurrentMaxKey < short.MaxValue)
            {

                if (m_CurrentMaxKey == -1)
                { // -1 is not a valid key, sending that destroys the UI effect
                    m_CurrentMaxKey++;
                }

                return new UnturnedUIEffectKey(m_CurrentMaxKey++); // increment key after getting the value
            }

            // all values have been used up already, grab one from released
            if (m_ReleasedKeys.Any())
            {
                UnturnedUIEffectKey result = m_ReleasedKeys[0];
                m_ReleasedKeys.RemoveAt(0);
                return result;
            }

            // no bindings and no released keys = reset and spawn key
            if (!m_Bindings.Any())
            {
                m_CurrentMaxKey = short.MinValue;
                return new UnturnedUIEffectKey(m_CurrentMaxKey++);
            }

            // no key otherwise
            return null;
        }

        /// <returns>List of key bindings for given component</returns>
        private List<UnturnedUIEffectKey> GetOrCreateBindingsFor(IOpenModComponent component)
        {
            if (m_Bindings.TryGetValue(component, out List<UnturnedUIEffectKey> list))
            { // return existing list
                return list;
            }

            // or create new list if one doesn't exist yet
            var newList = new List<UnturnedUIEffectKey>();
            m_Bindings[component] = newList;

            // register cleanup callback
            component.LifetimeScope.CurrentScopeEnding += ComponentScopeEnding;

            return newList;
        }

        private void ComponentScopeEnding(object sender, LifetimeScopeEndingEventArgs e)
        {
            // find bound component by it's lifetime scope
            IOpenModComponent? component = m_Bindings.Keys.FirstOrDefault(x => x.LifetimeScope == e.LifetimeScope);
            if (component == null)
            {
                return;
            }

            m_Logger.LogInformation("Component {ComponentId} lifetime scope ended. Releasing all its keys", component.OpenModComponentId);
            if (m_Bindings.TryGetValue(component, out List<UnturnedUIEffectKey> bindings))
            { // release all keys
                m_ReleasedKeys.AddRange(bindings);
            }

            // cleanup all component related stuff
            CleanupComponent(component);
        }

        private void CleanupComponent(IOpenModComponent component)
        {
            // remove bindings for this component
            m_Bindings.Remove(component);
            // unregister cleanup callback
            component.LifetimeScope.CurrentScopeEnding -= ComponentScopeEnding;
        }

        private void LogBiggestOffender(string callerComponentId)
        {
            KeyValuePair<IOpenModComponent, List<UnturnedUIEffectKey>> maxValueBinding = m_Bindings.MaxBy(x => x.Value.Count).First();
            m_Logger.LogWarning(
                "{CallingComponentId} tried to bind a UI effect key but there are none available. Component {MostKeysComponent} has {MostKeysCount} keys, consider disabling it",
                callerComponentId, maxValueBinding.Key, maxValueBinding.Value.Count.ToString()
            );
        }
    }
}