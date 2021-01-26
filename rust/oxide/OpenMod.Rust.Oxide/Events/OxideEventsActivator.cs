using System;
using System.Collections.Generic;
using Autofac;
using Microsoft.Extensions.Logging;
using OpenMod.Common.Helpers;
using OpenMod.Core.Ioc;
using Oxide.Core;

namespace OpenMod.Rust.Oxide.Events
{
    internal class OxideEventsActivator : IDisposable
    {
        private readonly ILifetimeScope m_LifetimeScope;
        private readonly ILogger<OxideEventsActivator> m_Logger;
        private readonly List<OxideEventsListenerBase> m_RustEventsListeners;

        public OxideEventsActivator(
            ILifetimeScope lifetimeScope,
            ILogger<OxideEventsActivator> logger)
        {
            m_LifetimeScope = lifetimeScope;
            m_Logger = logger;
            m_RustEventsListeners = new List<OxideEventsListenerBase>();
        }

        public void ActivateEventListeners()
        {
            m_Logger.LogTrace("Activating oxide events listeners");

            foreach (var type in FindListenerTypes())
            {
                var eventsListener = (OxideEventsListenerBase) ActivatorUtilitiesEx.CreateInstance(m_LifetimeScope, type);
                m_RustEventsListeners.Add(eventsListener);
            }

            foreach (var eventsListener in m_RustEventsListeners)
            {
                Interface.Oxide.RootPluginManager.AddPlugin(eventsListener);
            }
        }

        internal static IEnumerable<Type> FindListenerTypes()
        {
            return typeof(OxideEventsActivator).Assembly.FindTypes<OxideEventsListenerBase>(false);
        }

        public void Dispose()
        {
            m_Logger.LogTrace("Disposing oxide events listeners");

            foreach (var eventsListener in m_RustEventsListeners)
            {
                Interface.Oxide.RootPluginManager.RemovePlugin(eventsListener);
            }

            m_RustEventsListeners.Clear();
        }
    }
}
