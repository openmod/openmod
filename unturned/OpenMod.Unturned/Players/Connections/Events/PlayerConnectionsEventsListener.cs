extern alias JetBrainsAnnotations;
using System;
using JetBrainsAnnotations::JetBrains.Annotations;
using OpenMod.Unturned.Events;
using SDG.Unturned;
using Steamworks;

namespace OpenMod.Unturned.Players.Connections.Events
{
    [UsedImplicitly]
    internal class PlayerConnectionsEventsListener : UnturnedEventsListener
    {
        public PlayerConnectionsEventsListener(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override void Subscribe()
        {
            Provider.onServerConnected += OnPlayerConnected;
            Provider.onServerDisconnected += OnPlayerDisconnected;
            Provider.onRejectingPlayer += OnPlayerRejected;
        }

        public override void Unsubscribe()
        {
            Provider.onServerConnected -= OnPlayerConnected;
            Provider.onServerDisconnected -= OnPlayerDisconnected;
            Provider.onRejectingPlayer -= OnPlayerRejected;
        }

        private void OnPlayerConnected(CSteamID steamID)
        {
            var player = GetUnturnedPlayer(steamID)!;

            var @event = new UnturnedPlayerConnectedEvent(player);
            Emit(@event);
        }

        private void OnPlayerDisconnected(CSteamID steamID)
        {
            var player = GetUnturnedPlayer(steamID)!;

            var @event = new UnturnedPlayerDisconnectedEvent(player);
            Emit(@event);
        }

        private void OnPlayerRejected(CSteamID steamID, ESteamRejection rejection, string explanation)
        {
            var @event = new UnturnedPlayerRejectedEvent(steamID, rejection, explanation);
            Emit(@event);
        }
    }
}