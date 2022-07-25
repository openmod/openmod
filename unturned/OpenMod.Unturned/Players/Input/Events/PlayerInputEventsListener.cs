extern alias JetBrainsAnnotations;
using System;

using OpenMod.Unturned.Events;

using SDG.Unturned;

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
            PlayerInput.onPluginKeyTick += OnPluginKeyTick;
        }

        public override void Unsubscribe()
        {
            PlayerInput.onPluginKeyTick -= OnPluginKeyTick;
        }

        private readonly bool[] m_LastInputs = new bool[ControlsSettings.NUM_PLUGIN_KEYS];

        private void OnPluginKeyTick(Player nativePlayer, uint simulation, byte key, bool state)
        {
            if (key >= m_LastInputs.Length)
            {
                return;
            }

            if (m_LastInputs[key] == state)
            {
                return;
            }

            m_LastInputs[key] = state;

            var player = GetUnturnedPlayer(nativePlayer)!;
            var @event = new UnturnedPlayerPluginKeyStateChangedEvent(player, key, state);

            Emit(@event);
        }
    }
}
