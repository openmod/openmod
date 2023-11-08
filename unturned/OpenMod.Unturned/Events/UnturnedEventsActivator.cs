using HarmonyLib;
using Microsoft.Extensions.Logging;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using OpenMod.Common.Helpers;
using OpenMod.Core.Ioc;

namespace OpenMod.Unturned.Events
{
    internal class UnturnedEventsActivator : IDisposable
    {
        private readonly ILifetimeScope m_LifetimeScope;
        private readonly ILogger<UnturnedEventsActivator> m_Logger;
        private readonly List<IUnturnedEventsListener> m_UnturnedEventsListeners;

        public UnturnedEventsActivator(
            ILifetimeScope lifetimeScope,
            ILogger<UnturnedEventsActivator> logger)
        {
            m_LifetimeScope = lifetimeScope;
            m_Logger = logger;
            m_UnturnedEventsListeners = new List<IUnturnedEventsListener>();
        }

        public void ActivateEventListeners()
        {
            m_Logger.LogTrace("Activating unturned events listeners");

            var listenerTypes = GetType().Assembly.FindTypes<IUnturnedEventsListener>();
            foreach (var type in listenerTypes)
            {
                var eventsListener = (IUnturnedEventsListener)ActivatorUtilitiesEx.CreateInstance(m_LifetimeScope, type);
                m_UnturnedEventsListeners.Add(eventsListener);
            }

            foreach (var eventsListener in m_UnturnedEventsListeners)
            {
                eventsListener.Subscribe();
            }

            Provider.clients.Do(SubscribePlayer);

            Provider.onEnemyConnected += SubscribePlayer;
            Provider.onEnemyDisconnected += UnsubscribePlayer;
        }

        public void Dispose()
        {
            m_Logger.LogTrace("Disposing unturned events listeners");

            Provider.onEnemyConnected -= SubscribePlayer;
            Provider.onEnemyDisconnected -= UnsubscribePlayer;

            Provider.clients.Do(UnsubscribePlayer);

            foreach (var eventsListener in m_UnturnedEventsListeners)
            {
                eventsListener.Unsubscribe();
            }

            m_UnturnedEventsListeners.Clear();
        }

        private void SubscribePlayer(SteamPlayer player)
        {
            if (player.player == null) return;

            foreach (var eventsListener in m_UnturnedEventsListeners.OfType<IUnturnedPlayerEventsListener>())
            {
                eventsListener.SubscribePlayer(player.player);
            }
        }

        private void UnsubscribePlayer(SteamPlayer player)
        {
            if (player.player == null) return;

            foreach (var eventsListener in m_UnturnedEventsListeners.OfType<IUnturnedPlayerEventsListener>())
            {
                eventsListener.UnsubscribePlayer(player.player);
            }
        }
    }
}
