using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenMod.Common.Helpers;
using Oxide.Core;

namespace OpenMod.Rust.Oxide.Events
{
    internal class OxideEventsActivator : IDisposable
    {
        private readonly IServiceProvider m_ServiceProvider;
        private readonly ILogger<OxideEventsActivator> m_Logger;
        private readonly List<OxideEventsListenerBase> m_RustEventsListeners;

        public OxideEventsActivator(
            IServiceProvider serviceProvider,
            ILogger<OxideEventsActivator> logger)
        {
            m_ServiceProvider = serviceProvider;
            m_Logger = logger;
            m_RustEventsListeners = new List<OxideEventsListenerBase>();
        }

        public void ActivateEventListeners()
        {
            m_Logger.LogTrace("Activating oxide events listeners");

            foreach (Type type in FindListenerTypes())
            {
                var eventsListener = (OxideEventsListenerBase) ActivatorUtilities.CreateInstance(m_ServiceProvider, type);
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
