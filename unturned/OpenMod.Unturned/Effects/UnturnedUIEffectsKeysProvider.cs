using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using MoreLinq;

using OpenMod.API;
using OpenMod.API.Ioc;
using OpenMod.API.Prioritization;

namespace OpenMod.Unturned.Effects
{
    [OpenModInternal]
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    public class UnturnedUIEffectsKeysProvider : IUnturnedUIEffectsKeysProvider
    {
        private readonly ILogger<UnturnedUIEffectsKeysProvider> m_Logger;

        private short m_CurrentMaxKey = short.MinValue;

        private readonly Dictionary<string, List<UnturnedUIEffectKey>> m_Bindings = new();

        private readonly List<UnturnedUIEffectKey> m_ReleasedKeys = new();

        public UnturnedUIEffectsKeysProvider(ILogger<UnturnedUIEffectsKeysProvider> logger)
        {
            m_Logger = logger;
        }

        public UnturnedUIEffectKey BindKey(IOpenModComponent component)
        {
            if (!component.IsComponentAlive)
            {
                m_Logger.LogDebug("{ComponentId} tried to bind a UI effect key but the component is not alive", component.OpenModComponentId);
                return UnturnedUIEffectKey.Invalid;
            }

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
            if (!component.IsComponentAlive)
            {
                m_Logger.LogDebug(
                    "{ComponentId} tried to bind a UI effect keys but the component is not alive",
                    component.OpenModComponentId);
                return Enumerable.Repeat(UnturnedUIEffectKey.Invalid, amount);
            }

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

            List<UnturnedUIEffectKey> bindings = GetOrCreateBindingsFor(component);
            bindings.AddRange(result.Where(x => x != UnturnedUIEffectKey.Invalid));

            if (logOffender)
            {
                LogBiggestOffender(component.OpenModComponentId);
            }

            return result;
        }

        public bool ReleaseKey(IOpenModComponent component, UnturnedUIEffectKey key)
        {
            if (!m_Bindings.TryGetValue(component.OpenModComponentId, out List<UnturnedUIEffectKey>? bindings))
            {
                return false;
            }

            // try to remove key from bindings list
            bool result = bindings.Remove(key);
            if (result)
            {
                m_ReleasedKeys.Add(key);
            }

            // remove empty bindings entry
            if (!bindings.Any())
            {
                m_Bindings.Remove(component.OpenModComponentId);
            }

            return result;
        }

        public void ReleaseAllKeys(IOpenModComponent component)
        {
            if (m_Bindings.TryGetValue(component.OpenModComponentId, out List<UnturnedUIEffectKey>? bindings))
            {
                m_ReleasedKeys.AddRange(bindings);
                m_Bindings.Remove(component.OpenModComponentId);
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

        private List<UnturnedUIEffectKey> GetOrCreateBindingsFor(IOpenModComponent component)
        {
            string componentId = component.OpenModComponentId;
            if (m_Bindings.TryGetValue(componentId, out List<UnturnedUIEffectKey> list))
            {
                return list;
            }

            list = new List<UnturnedUIEffectKey>();
            m_Bindings[componentId] = list;

            component.LifetimeScope.CurrentScopeEnding += (_, _) => { OnComponentUnload(componentId); };

            return list;
        }

        private void OnComponentUnload(string componentId)
        {
            m_Logger.LogInformation("Component {ComponentId} unloaded. Releasing all keys", componentId);
            if (m_Bindings.TryGetValue(componentId, out List<UnturnedUIEffectKey>? bindings))
            {
                m_ReleasedKeys.AddRange(bindings);
                m_Bindings.Remove(componentId);
            }
        }

        private void LogBiggestOffender(string componentId)
        {
            KeyValuePair<string, List<UnturnedUIEffectKey>> maxValueBinding =
                m_Bindings.MaxBy(x => x.Value.Count).First();
            m_Logger.LogWarning(
                "{CallingComponentId} tried to bind a UI effect key but there are none available. Component {MostKeysComponent} has {MostKeysCount} keys, consider disabling it",
                componentId, maxValueBinding.Key, maxValueBinding.Value.Count.ToString());
        }
    }
}