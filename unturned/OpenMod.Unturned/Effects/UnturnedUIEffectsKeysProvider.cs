using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using MoreLinq;

using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Ioc;
using OpenMod.API.Plugins;
using OpenMod.API.Prioritization;
using OpenMod.Core.Plugins.Events;

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

        public UnturnedUIEffectsKeysProvider(
            ILogger<UnturnedUIEffectsKeysProvider> logger,
            IRuntime runtime,
            IEventBus eventBus)
        {
            m_Logger = logger;
            eventBus.Subscribe<PluginUnloadedEvent>(runtime, (_, _, ev) => OnPluginUnloaded(ev));
        }

        private Task OnPluginUnloaded(PluginUnloadedEvent ev)
        {
            string pluginId = ev.Plugin.OpenModComponentId;
            if (m_Bindings.ContainsKey(pluginId))
            {
                List<UnturnedUIEffectKey> bindings = m_Bindings[pluginId];
                m_ReleasedKeys.AddRange(bindings);
                m_Bindings.Remove(pluginId);
            }

            return Task.CompletedTask;
        }

        public UnturnedUIEffectKey BindKey(IOpenModPlugin plugin)
        {
            if (!plugin.IsComponentAlive)
            {
                m_Logger.LogDebug("{ComponentId} tried to bind a UI effect key but the plugin is not alive", plugin.OpenModComponentId);
                return UnturnedUIEffectKey.Invalid;
            }

            UnturnedUIEffectKey? result = GetNewKey();

            if (result != null)
            { // if we got a key, bind it

                List<UnturnedUIEffectKey> bindings = GetBindingFor(plugin.OpenModComponentId);
                bindings.Add(result.Value);

                return result.Value;
            }

            // all keys are taken, log biggest offender
            LogBiggestOffender(plugin.OpenModComponentId);

            return UnturnedUIEffectKey.Invalid;
        }

        public IEnumerable<UnturnedUIEffectKey> BindKeys(IOpenModPlugin plugin, int amount)
        {
            if (!plugin.IsComponentAlive)
            {
                m_Logger.LogDebug(
                    "{ComponentId} tried to bind a UI effect keys but the plugin is not alive",
                    plugin.OpenModComponentId);
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

            List<UnturnedUIEffectKey> bindings = GetBindingFor(plugin.OpenModComponentId);
            bindings.AddRange(result.Where(x => x != UnturnedUIEffectKey.Invalid));

            if (logOffender)
            {
                LogBiggestOffender(plugin.OpenModComponentId);
            }

            return result;
        }

        public bool ReleaseKey(IOpenModPlugin plugin, UnturnedUIEffectKey key)
        {
            List<UnturnedUIEffectKey> bindings = GetBindingFor(plugin.OpenModComponentId);

            bool result = bindings.Remove(key);
            if (result)
            {
                m_ReleasedKeys.Add(key);
            }

            return result;
        }

        public void ReleaseAllKeys(IOpenModPlugin plugin)
        {
            List<UnturnedUIEffectKey> bindings = GetBindingFor(plugin.OpenModComponentId);
            m_ReleasedKeys.AddRange(bindings);
            m_Bindings.Remove(plugin.OpenModComponentId);
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

        private List<UnturnedUIEffectKey> GetBindingFor(string pluginId)
        {
            if (m_Bindings.ContainsKey(pluginId))
            {
                return m_Bindings[pluginId];
            }

            var list = new List<UnturnedUIEffectKey>();
            m_Bindings.Add(pluginId, list);

            return list;
        }

        private void LogBiggestOffender(string pluginId)
        {
            KeyValuePair<string, List<UnturnedUIEffectKey>> maxValueBinding =
                m_Bindings.MaxBy(x => x.Value.Count).First();
            m_Logger.LogWarning(
                "{CallingComponentId} tried to bind a UI effect key but there are none available. Plugin {MostKeysComponent} has {MostKeysCount} keys, consider disabling it",
                pluginId, maxValueBinding.Key, maxValueBinding.Value.Count.ToString());
        }
    }
}