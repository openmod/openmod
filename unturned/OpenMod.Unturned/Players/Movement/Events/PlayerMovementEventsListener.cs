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
            OnGestureUpdated += Events_OnGestureUpdated;
            OnTeleporting += Events_OnTeleporting;
        }

        public override void Unsubscribe()
        {
            OnGestureUpdated -= Events_OnGestureUpdated;
            OnTeleporting -= Events_OnTeleporting;
        }

        public override void SubscribePlayer(Player player)
        {
            player.stance.onStanceUpdated += () => OnStanceUpdated(player);
        }

        public override void UnsubscribePlayer(Player player)
        {
            player.stance.onStanceUpdated -= () => OnStanceUpdated(player);
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

        private void Events_OnGestureUpdated(Player nativePlayer, EPlayerGesture gesture)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerGestureUpdatedEvent @event = new UnturnedPlayerGestureUpdatedEvent(player, gesture);

            Emit(@event);
        }

        private delegate void GestureUpdated(Player player, EPlayerGesture gesture);
        private static event GestureUpdated OnGestureUpdated;

        private delegate void Teleporting(Player player,
            ref Vector3 position, ref float yaw, out bool cancel);
        private static event Teleporting OnTeleporting;

        [HarmonyPatch]
        private class Patches
        {
            [HarmonyPatch(typeof(PlayerAnimator), "sendGesture")]
            [HarmonyPostfix]
            private static void SendGesture(PlayerAnimator __instance, EPlayerGesture gesture)
            {
                if (gesture == EPlayerGesture.REST_START && __instance.player.stance.stance != EPlayerStance.CROUCH)
                {
                    return;
                }

                OnGestureUpdated?.Invoke(__instance.player, gesture);
            }

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