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
        }

        public override void Unsubscribe()
        {
            ItemManager.onTakeItemRequested -= OnTakeItemRequested;
        }

        public override void SubscribePlayer(Player player)
        {
            player.inventory.onDropItemRequested += OnDropItemRequested;
            player.inventory.onInventoryResized +=
                (page, width, height) => OnInventoryResized(player, page, width, height);
            player.inventory.onInventoryStored += () => OnInventoryStored(player);
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
            player.inventory.onInventoryStored -= () => OnInventoryStored(player);
            player.inventory.onInventoryStateUpdated -= () => OnInventoryStateUpdated(player);
            player.inventory.onInventoryAdded -= (page, index, jar) => OnInventoryAdded(player, page, index, jar);
            player.inventory.onInventoryRemoved -= (page, index, jar) => OnInventoryRemoved(player, page, index, jar);
            player.inventory.onInventoryUpdated -= (page, index, jar) => OnInventoryUpdated(player, page, index, jar);
        }

        private void OnTakeItemRequested(Player nativePlayer, byte x, byte y, uint instanceID, byte to_x, byte to_y, byte to_rot,
            byte to_page, ItemData itemData, ref bool shouldAllow)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerPickupItemEvent @event = new UnturnedPlayerPickupItemEvent(player, x, y, instanceID,
                to_x, to_y, to_rot, to_page, itemData);

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
        }

        private void OnDropItemRequested(PlayerInventory inventory, Item item, ref bool shouldAllow)
        {
            UnturnedPlayer player = GetUnturnedPlayer(inventory.player);

            UnturnedPlayerDropItemEvent @event = new UnturnedPlayerDropItemEvent(player, item);

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
        }

        private void OnInventoryResized(Player nativePlayer, byte page, byte width, byte height)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerInventoryResizeEvent @event = new UnturnedPlayerInventoryResizeEvent(player, page, width, height);

            Emit(@event);

            if (page == PlayerInventory.STORAGE && width == 0 && height == 0)
            {
                UnturnedPlayerCloseStorageEvent closeStorageEvent = new UnturnedPlayerCloseStorageEvent(player);

                Emit(closeStorageEvent);
            }
        }

        private void OnInventoryStored(Player nativePlayer)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerOpenStorageEvent @event = new UnturnedPlayerOpenStorageEvent(player);

            Emit(@event);
        }

        private void OnInventoryStateUpdated(Player nativePlayer)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerInventoryUpdateEvent @event = new UnturnedPlayerInventoryUpdateEvent(player);

            Emit(@event);
        }

        private void OnInventoryAdded(Player nativePlayer, byte page, byte index, ItemJar jar)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerItemAddEvent @event = new UnturnedPlayerItemAddEvent(player, page, index, jar);

            Emit(@event);
        }

        private void OnInventoryRemoved(Player nativePlayer, byte page, byte index, ItemJar jar)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerItemRemoveEvent @event = new UnturnedPlayerItemRemoveEvent(player, page, index, jar);

            Emit(@event);
        }

        private void OnInventoryUpdated(Player nativePlayer, byte page, byte index, ItemJar jar)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerItemUpdateEvent @event = new UnturnedPlayerItemUpdateEvent(player, page, index, jar);

            Emit(@event);
        }
    }
}
