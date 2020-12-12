using HarmonyLib;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenMod.Core.Helpers;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using OpenMod.Common.Helpers;

namespace OpenMod.Unturned.Events
{
    internal class UnturnedEventsActivator : IDisposable
    {
        private readonly IServiceProvider m_ServiceProvider;
        private readonly ILogger<UnturnedEventsActivator> m_Logger;
        private readonly List<IUnturnedEventsListener> m_UnturnedEventsListeners;

        public UnturnedEventsActivator(
            IServiceProvider serviceProvider,
            ILogger<UnturnedEventsActivator> logger)
        {
            m_ServiceProvider = serviceProvider;
            m_Logger = logger;
            m_UnturnedEventsListeners = new List<IUnturnedEventsListener>();
        }

        public void ActivateEventListeners()
        {
            m_Logger.LogTrace("Activating unturned events listeners");

            List<Type> listenerTypes = GetType().Assembly.FindTypes<IUnturnedEventsListener>(false).ToList();

            foreach (Type type in listenerTypes)
            {
                IUnturnedEventsListener eventsListener = (IUnturnedEventsListener)ActivatorUtilities.CreateInstance(m_ServiceProvider, type);

                m_UnturnedEventsListeners.Add(eventsListener);
            }

            foreach (IUnturnedEventsListener eventsListener in m_UnturnedEventsListeners)
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

            foreach (IUnturnedEventsListener eventsListener in m_UnturnedEventsListeners)
            {
                eventsListener.Unsubscribe();
            }

            m_UnturnedEventsListeners.Clear();
        }

        private void SubscribePlayer(SteamPlayer player)
        {
            if (player == null || player.player == null) return;

            foreach (IUnturnedPlayerEventsListener eventsListener in m_UnturnedEventsListeners.OfType<IUnturnedPlayerEventsListener>())
            {
                eventsListener.SubscribePlayer(player.player);
            }
        }

        private void UnsubscribePlayer(SteamPlayer player)
        {
            if (player == null || player.player == null) return;

            foreach (IUnturnedPlayerEventsListener eventsListener in m_UnturnedEventsListeners.OfType<IUnturnedPlayerEventsListener>())
            {
                eventsListener.UnsubscribePlayer(player.player);
            }
        }
    }
}
