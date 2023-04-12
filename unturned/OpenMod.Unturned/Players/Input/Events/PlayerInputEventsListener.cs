extern alias JetBrainsAnnotations;
using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrainsAnnotations::JetBrains.Annotations;
using OpenMod.Unturned.Events;
using SDG.Unturned;

namespace OpenMod.Unturned.Players.Input.Events
{
    [UsedImplicitly]
    internal class PlayerInputEventsListener : UnturnedEventsListener
    {
        public PlayerInputEventsListener(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override void Subscribe()
        {
            PlayerInput.onPluginKeyTick += OnPluginKeyTick;
            OnPlayerInputted += Events_OnPlayerInputted;
        }

        public override void Unsubscribe()
        {
            PlayerInput.onPluginKeyTick -= OnPluginKeyTick;
            OnPlayerInputted -= Events_OnPlayerInputted;
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

        private void Events_OnPlayerInputted(PlayerInput input, ref InputInfo? inputinfo, bool doOcclusionCheck, ERaycastInfoUsage usage)
        {
            var player = GetUnturnedPlayer(input.player)!;
            var @event = new UnturnedPlayerInputtingEvent(player, inputinfo, doOcclusionCheck, usage);

            Emit(@event);

            inputinfo = @event.InputInfo;
        }

        private delegate void PlayerInputted(PlayerInput input, ref InputInfo? inputInfo, bool doOcclusionCheck, ERaycastInfoUsage usage);

        private static event PlayerInputted? OnPlayerInputted;

        [UsedImplicitly]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        [HarmonyPatch]
        public static class Patches
        {
            [UsedImplicitly]
            [HarmonyPatch(typeof(PlayerInput), nameof(PlayerInput.getInput), typeof(bool), typeof(ERaycastInfoUsage))]
            [HarmonyPostfix]
            public static void PostGetInput(PlayerInput __instance, ref InputInfo __result, bool doOcclusionCheck, ERaycastInfoUsage usage)
            {
                OnPlayerInputted?.Invoke(__instance, ref __result!, doOcclusionCheck, usage);
            }
        }
    }
}
