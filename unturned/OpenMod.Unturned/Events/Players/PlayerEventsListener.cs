using HarmonyLib;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Users;
using OpenMod.Extensions.Games.Abstractions.Entities;
using OpenMod.Unturned.Entities;
using SDG.Unturned;
using Steamworks;
using System.Linq;
using UnityEngine;

namespace OpenMod.Unturned.Events.Players
{
    internal class PlayerEventsListener : UnturnedPlayerEventsListener
    {
        public PlayerEventsListener(IOpenModHost openModHost,
            IEventBus eventBus,
            IUserManager userManager) : base(openModHost, eventBus, userManager)
        {

        }

        public override void Subscribe()
        {
            OnTeleport += Events_OnTeleport;

            UseableConsumeable.onPerformingAid += OnPerformingAid;
        }

        public override void Unsubscribe()
        {
            OnTeleport -= Events_OnTeleport;

            UseableConsumeable.onPerformingAid -= OnPerformingAid;
        }

        public override void SubscribePlayer(Player player)
        {
            player.stance.onStanceUpdated += () => OnStanceUpdated(player);
        }

        public override void UnsubscribePlayer(Player player)
        {
            player.stance.onStanceUpdated -= () => OnStanceUpdated(player);
        }

        private void Events_OnTeleport(Player nativePlayer, ref Vector3 position, ref float yaw, out bool cancel)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerTeleportEvent @event = new UnturnedPlayerTeleportEvent(player, position, yaw);

            Emit(@event);

            position = @event.Position;
            yaw = @event.Yaw;
            cancel = @event.IsCancelled;
        }

        private void OnPerformingAid(Player nativeInstigator, Player nativeTarget, ItemConsumeableAsset asset, ref bool shouldAllow)
        {
            UnturnedPlayer instigator = GetUnturnedPlayer(nativeInstigator);
            UnturnedPlayer target = GetUnturnedPlayer(nativeTarget);

            UnturnedPlayerPerformAidEvent @event = new UnturnedPlayerPerformAidEvent(instigator, target, asset);

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
        }

        private void OnStanceUpdated(Player nativePlayer)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerStanceUpdateEvent @event = new UnturnedPlayerStanceUpdateEvent(player);

            Emit(@event);
        }

        private delegate void TeleportHandler(Player player,
            ref Vector3 position, ref float yaw, out bool cancel);
        private static event TeleportHandler OnTeleport;

        [HarmonyPatch]
        private class Patches
        {
            [HarmonyPatch(typeof(Player), "teleportToLocationUnsafe")]
            [HarmonyPrefix]
            private static bool TeleportToLocationUnsafe(Player __instance, ref Vector3 position, ref float yaw)
            {
                bool cancel = false;

                OnTeleport?.Invoke(__instance, ref position, ref yaw, out cancel);

                return !cancel;
            }
        }
    }
}
