using System;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using OpenMod.Unturned.Events;
using OpenMod.Unturned.Patching;
using SDG.Unturned;

namespace OpenMod.Unturned.Players.Inventory.Events
{
    [UsedImplicitly]
    internal class PlayerInventoryEventsListener : UnturnedPlayerEventsListener
    {
        public PlayerInventoryEventsListener(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            SubscribePlayer(
                static player => ref player.inventory.onDropItemRequested,
                _ => OnDropItemRequested
            );
            SubscribePlayer(
                static player => ref player.inventory.onInventoryResized,
                player => (page, width, height) => OnInventoryResized(player, page, width, height)
            );
            SubscribePlayer(
                static player => ref player.inventory.onInventoryStateUpdated,
                player => () => OnInventoryStateUpdated(player)
            );
            SubscribePlayer(
                static player => ref player.inventory.onInventoryAdded,
                player => (page, index, jar) => OnInventoryAdded(player, page, index, jar)
            );
            SubscribePlayer(
                static player => ref player.inventory.onInventoryRemoved,
                player => (page, index, jar) => OnInventoryRemoved(player, page, index, jar)
            );
            SubscribePlayer(
                static player => ref player.inventory.onInventoryUpdated,
                player => (page, index, jar) => OnInventoryUpdated(player, page, index, jar)
            );
        }

        public override void Subscribe()
        {
            ItemManager.onTakeItemRequested += OnTakeItemRequested;
            OnOpenedStorage += Events_OnOpenedStorage;
        }

        public override void Unsubscribe()
        {
            ItemManager.onTakeItemRequested -= OnTakeItemRequested;
            OnOpenedStorage -= Events_OnOpenedStorage;
        }

        // ReSharper disable InconsistentNaming
        private void OnTakeItemRequested(Player nativePlayer, byte x, byte y, uint instanceID, byte to_x, byte to_y, byte to_rot,
            byte to_page, ItemData itemData, ref bool shouldAllow)
            // ReSharper restore InconsistentNaming
        {
            var player = GetUnturnedPlayer(nativePlayer)!;

            var @event = new UnturnedPlayerTakingItemEvent(player, x, y, instanceID,
                to_x, to_y, to_rot, to_page, itemData)
            {
                IsCancelled = !shouldAllow
            };

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
        }

        private void OnDropItemRequested(PlayerInventory inventory, Item item, ref bool shouldAllow)
        {
            var player = GetUnturnedPlayer(inventory.player)!;

            var @event = new UnturnedPlayerDroppedItemEvent(player, item)
            {
                IsCancelled = !shouldAllow
            };

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
        }

        private void Events_OnOpenedStorage(Player nativePlayer)
        {
            var player = GetUnturnedPlayer(nativePlayer)!;

            var @event = new UnturnedPlayerOpenedStorageEvent(player);

            Emit(@event);
        }

        private void OnInventoryResized(Player nativePlayer, byte page, byte width, byte height)
        {
            var player = GetUnturnedPlayer(nativePlayer)!;

            var @event = new UnturnedPlayerInventoryResizedEvent(player, page, width, height);

            Emit(@event);

            if (page == PlayerInventory.STORAGE && width == 0 && height == 0)
            {
                var closedStorageEvent = new UnturnedPlayerClosedStorageEvent(player);

                Emit(closedStorageEvent);
            }
        }

        private void OnInventoryStateUpdated(Player nativePlayer)
        {
            var player = GetUnturnedPlayer(nativePlayer)!;

            var @event = new UnturnedPlayerInventoryUpdatedEvent(player);

            Emit(@event);
        }

        private void OnInventoryAdded(Player nativePlayer, byte page, byte index, ItemJar jar)
        {
            var player = GetUnturnedPlayer(nativePlayer)!;

            var @event = new UnturnedPlayerItemAddedEvent(player, page, index, jar);

            Emit(@event);
        }

        private void OnInventoryRemoved(Player nativePlayer, byte page, byte index, ItemJar jar)
        {
            var player = GetUnturnedPlayer(nativePlayer)!;

            var @event = new UnturnedPlayerItemRemovedEvent(player, page, index, jar);

            Emit(@event);
        }

        private void OnInventoryUpdated(Player nativePlayer, byte page, byte index, ItemJar jar)
        {
            var player = GetUnturnedPlayer(nativePlayer)!;

            var @event = new UnturnedPlayerItemUpdatedEvent(player, page, index, jar);

            Emit(@event);
        }

        private delegate void OpenedStorage(Player player);
        private static event OpenedStorage? OnOpenedStorage;

        [UsedImplicitly]
        [HarmonyPatch]
        internal static class Patches
        {
            [HarmonyCleanup]
            public static Exception? Cleanup(Exception ex, MethodBase original)
            {
                HarmonyExceptionHandler.ReportCleanupException(typeof(Patches), ex, original);
                return null;
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(PlayerInventory), nameof(PlayerInventory.openStorage))]
            [HarmonyPostfix]
            // ReSharper disable once InconsistentNaming
            public static void OpenStorage(PlayerInventory __instance)
            {
                OnOpenedStorage?.Invoke(__instance.player);
            }
        }
    }
}
