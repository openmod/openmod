using HarmonyLib;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Users;
using OpenMod.Unturned.Events;
using SDG.Unturned;

namespace OpenMod.Unturned.Players.Events.Inventory
{
    internal class PlayerInventoryEventsListener : UnturnedPlayerEventsListener
    {
        public PlayerInventoryEventsListener(IOpenModHost openModHost,
            IEventBus eventBus,
            IUserManager userManager) : base(openModHost, eventBus, userManager)
        {

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

        public override void SubscribePlayer(Player player)
        {
            player.inventory.onDropItemRequested += OnDropItemRequested;
            player.inventory.onInventoryResized +=
                (page, width, height) => OnInventoryResized(player, page, width, height);
            player.inventory.onInventoryStateUpdated += () => OnInventoryStateUpdated(player);
            player.inventory.onInventoryAdded += (page, index, jar) => OnInventoryAdded(player, page, index, jar);
            player.inventory.onInventoryRemoved += (page, index, jar) => OnInventoryRemoved(player, page, index, jar);
            player.inventory.onInventoryUpdated += (page, index, jar) => OnInventoryUpdated(player, page, index, jar);
        }

        public override void UnsubscribePlayer(Player player)
        {
            player.inventory.onDropItemRequested -= OnDropItemRequested;
            player.inventory.onInventoryResized -=
                (page, width, height) => OnInventoryResized(player, page, width, height);
            player.inventory.onInventoryStateUpdated -= () => OnInventoryStateUpdated(player);
            player.inventory.onInventoryAdded -= (page, index, jar) => OnInventoryAdded(player, page, index, jar);
            player.inventory.onInventoryRemoved -= (page, index, jar) => OnInventoryRemoved(player, page, index, jar);
            player.inventory.onInventoryUpdated -= (page, index, jar) => OnInventoryUpdated(player, page, index, jar);
        }

        private void OnTakeItemRequested(Player nativePlayer, byte x, byte y, uint instanceID, byte to_x, byte to_y, byte to_rot,
            byte to_page, ItemData itemData, ref bool shouldAllow)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerTakingItemEvent @event = new UnturnedPlayerTakingItemEvent(player, x, y, instanceID,
                to_x, to_y, to_rot, to_page, itemData);

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
        }

        private void OnDropItemRequested(PlayerInventory inventory, Item item, ref bool shouldAllow)
        {
            UnturnedPlayer player = GetUnturnedPlayer(inventory.player);

            UnturnedPlayerDroppedItemEvent @event = new UnturnedPlayerDroppedItemEvent(player, item);

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
        }

        private void Events_OnOpenedStorage(Player nativePlayer)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerOpenedStorageEvent @event = new UnturnedPlayerOpenedStorageEvent(player);

            Emit(@event);
        }

        private void OnInventoryResized(Player nativePlayer, byte page, byte width, byte height)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerInventoryResizedEvent @event = new UnturnedPlayerInventoryResizedEvent(player, page, width, height);

            Emit(@event);

            if (page == PlayerInventory.STORAGE && width == 0 && height == 0)
            {
                UnturnedPlayerClosedStorageEvent closedStorageEvent = new UnturnedPlayerClosedStorageEvent(player);

                Emit(closedStorageEvent);
            }
        }

        private void OnInventoryStateUpdated(Player nativePlayer)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerInventoryUpdatedEvent @event = new UnturnedPlayerInventoryUpdatedEvent(player);

            Emit(@event);
        }

        private void OnInventoryAdded(Player nativePlayer, byte page, byte index, ItemJar jar)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerItemAddedEvent @event = new UnturnedPlayerItemAddedEvent(player, page, index, jar);

            Emit(@event);
        }

        private void OnInventoryRemoved(Player nativePlayer, byte page, byte index, ItemJar jar)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerItemRemovedEvent @event = new UnturnedPlayerItemRemovedEvent(player, page, index, jar);

            Emit(@event);
        }

        private void OnInventoryUpdated(Player nativePlayer, byte page, byte index, ItemJar jar)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerItemUpdatedEvent @event = new UnturnedPlayerItemUpdatedEvent(player, page, index, jar);

            Emit(@event);
        }

        private delegate void OpenedStorage(Player player);
        private static event OpenedStorage OnOpenedStorage;

        [HarmonyPatch]
        private class Patches
        {
            [HarmonyPatch(typeof(PlayerInventory), "openStorage")]
            [HarmonyPostfix]
            private static void OpenStorage(PlayerInventory __instance)
            {
                OnOpenedStorage?.Invoke(__instance.player);
            }
        }
    }
}
