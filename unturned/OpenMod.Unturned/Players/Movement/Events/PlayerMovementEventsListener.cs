using HarmonyLib;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Users;
using OpenMod.UnityEngine.Extensions;
using OpenMod.Unturned.Events;
using SDG.Unturned;
using UnityEngine;

namespace OpenMod.Unturned.Players.Movement.Events
{
    internal class PlayerMovementEventsListener : UnturnedPlayerEventsListener
    {
        public PlayerMovementEventsListener(IOpenModHost openModHost,
            IEventBus eventBus,
            IUserManager userManager) : base(openModHost, eventBus, userManager)
        {

        }

        public override void Subscribe()
        {
            OnTeleporting += Events_OnTeleporting;
        }

        public override void Unsubscribe()
        {
            OnTeleporting -= Events_OnTeleporting;
        }

        public override void SubscribePlayer(Player player)
        {
            player.stance.onStanceUpdated += () => OnStanceUpdated(player);
            player.animator.onGestureUpdated += () => OnGestureUpdated(player);
        }

        public override void UnsubscribePlayer(Player player)
        {
            player.stance.onStanceUpdated -= () => OnStanceUpdated(player);
            player.animator.onGestureUpdated -= () => OnGestureUpdated(player);
        }

        private void Events_OnTeleporting(Player nativePlayer, ref Vector3 position, ref float yaw, out bool cancel)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerTeleportingEvent @event = new UnturnedPlayerTeleportingEvent(player, position.ToSystemVector(), yaw);

            Emit(@event);

            position = @event.Position.ToUnityVector();
            yaw = @event.Yaw;
            cancel = @event.IsCancelled;
        }

        private void OnStanceUpdated(Player nativePlayer)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerStanceUpdatedEvent @event = new UnturnedPlayerStanceUpdatedEvent(player);

            Emit(@event);
        }

        private void OnGestureUpdated(Player nativePlayer)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerGestureUpdatedEvent @event = new UnturnedPlayerGestureUpdatedEvent(player);

            Emit(@event);
        }

        private delegate void Teleporting(Player player,
            ref Vector3 position, ref float yaw, out bool cancel);
        private static event Teleporting OnTeleporting;

        [HarmonyPatch]
        private class Patches
        {
            [HarmonyPatch(typeof(Player), "teleportToLocationUnsafe")]
            [HarmonyPrefix]
            private static bool TeleportToLocationUnsafe(Player __instance, ref Vector3 position, ref float yaw)
            {
                bool cancel = false;

                OnTeleporting?.Invoke(__instance, ref position, ref yaw, out cancel);

                return !cancel;
            }
        }
    }
}