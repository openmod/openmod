extern alias JetBrainsAnnotations;
using System;
using System.Collections.Generic;

using OpenMod.Unturned.Events;

using SDG.Unturned;

using Steamworks;

namespace OpenMod.Unturned.Players.Input.Events
{
    [JetBrainsAnnotations::JetBrains.Annotations.UsedImplicitlyAttribute]
    internal class PlayerInputEventsListener : UnturnedEventsListener
    {
        public PlayerInputEventsListener(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override void Subscribe()
        {
            Provider.onServerConnected += OnServerConnected;
            Provider.onServerDisconnected += OnServerDisconnected;
            PlayerInput.onPluginKeyTick += OnPluginKeyTick;
        }

        public override void Unsubscribe()
        {
            Provider.onServerConnected -= OnServerConnected;
            Provider.onServerDisconnected -= OnServerDisconnected;
            PlayerInput.onPluginKeyTick -= OnPluginKeyTick;
        }

        private void OnServerConnected(CSteamID steamID)
        {
            m_LastInputs.Add(steamID, new bool[ControlsSettings.NUM_PLUGIN_KEYS]);
        }

        private void OnServerDisconnected(CSteamID steamID)
        {
            m_LastInputs.Remove(steamID);
        }

        private readonly Dictionary<CSteamID, bool[]> m_LastInputs = new();

        private void OnPluginKeyTick(Player nativePlayer, uint simulation, byte key, bool state)
        {
            if (key >= ControlsSettings.NUM_PLUGIN_KEYS)
            {
                return;
            }

            var playerSteamId = nativePlayer.channel.owner.playerID.steamID;

            if (m_LastInputs[playerSteamId][key] == state)
            {
                return;
            }

            m_LastInputs[playerSteamId][key] = state;

            var player = GetUnturnedPlayer(nativePlayer)!;
            var @event = new UnturnedPlayerPluginKeyStateChangedEvent(player, key, state);

            Emit(@event);
        }
    }
}
