using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using OpenMod.Unturned.Events;
using SDG.Unturned;
using Steamworks;

namespace OpenMod.Unturned.Players.Input.Events
{
    [UsedImplicitly]
    internal class PlayerInputEventsListener : UnturnedPlayerEventsListener
    {
        private readonly Dictionary<CSteamID, bool[]> m_LastInputs = new();

        public PlayerInputEventsListener(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override void Subscribe()
        {
            PlayerInput.onPluginKeyTick += OnPluginKeyTick;
        }

        public override void Unsubscribe()
        {
            PlayerInput.onPluginKeyTick -= OnPluginKeyTick;
        }

        public override void SubscribePlayer(Player player)
        {
            base.SubscribePlayer(player);

            var playerSteamId = GetSteamIdOf(player);
            m_LastInputs.Add(playerSteamId, new bool[ControlsSettings.NUM_PLUGIN_KEYS]);
        }

        public override void UnsubscribePlayer(Player player)
        {
            base.UnsubscribePlayer(player);

            var playerSteamId = GetSteamIdOf(player);
            m_LastInputs.Remove(playerSteamId);
        }

        private void OnPluginKeyTick(Player nativePlayer, uint simulation, byte key, bool state)
        {
            if (key >= ControlsSettings.NUM_PLUGIN_KEYS)
            {
                return;
            }

            var playerSteamId = GetSteamIdOf(nativePlayer);

            if (!m_LastInputs.TryGetValue(playerSteamId, out var lastInputs))
            {
                return;
            }

            if (lastInputs[key] == state)
            {
                return;
            }

            lastInputs[key] = state;

            var player = GetUnturnedPlayer(nativePlayer)!;
            var @event = new UnturnedPlayerPluginKeyStateChangedEvent(player, key, state);

            Emit(@event);
        }

        private CSteamID GetSteamIdOf(Player nativePlayer)
        {
            return nativePlayer.channel.owner.playerID.steamID;
        }
    }
}
