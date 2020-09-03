using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Users;
using OpenMod.Unturned.Events;
using OpenMod.Unturned.Items;
using SDG.Unturned;

namespace OpenMod.Unturned.Players.Events.Equipment
{
    internal class PlayerEquipmentEventsListener : UnturnedPlayerEventsListener
    {
        public PlayerEquipmentEventsListener(IOpenModHost openModHost,
            IEventBus eventBus,
            IUserManager userManager) : base(openModHost, eventBus, userManager)
        {

        }

        public override void Subscribe()
        {

        }

        public override void Unsubscribe()
        {

        }

        public override void SubscribePlayer(Player player)
        {
            player.equipment.onEquipRequested += OnEquipRequested;
            player.equipment.onDequipRequested += OnDequipRequested;
        }

        public override void UnsubscribePlayer(Player player)
        {
            player.equipment.onEquipRequested -= OnEquipRequested;
            player.equipment.onDequipRequested -= OnDequipRequested;
        }

        private void OnEquipRequested(PlayerEquipment equipment, ItemJar jar, ItemAsset asset, ref bool shouldAllow)
        {
            UnturnedPlayer player = GetUnturnedPlayer(equipment.player);

            UnturnedPlayerItemEquipEvent @event = new UnturnedPlayerItemEquipEvent(player, new UnturnedItem(jar.item));

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
        }

        private void OnDequipRequested(PlayerEquipment equipment, ref bool shouldAllow)
        {
            UnturnedPlayer player = GetUnturnedPlayer(equipment.player);

            PlayerInventory inv = player.Player.inventory;

            byte page = equipment.equippedPage;

            byte index = inv.getIndex(page, equipment.equipped_x, equipment.equipped_y);

            ItemJar jar = inv.getItem(page, index);

            if (jar?.item == null) return;

            UnturnedPlayerItemDequipEvent @event = new UnturnedPlayerItemDequipEvent(player, new UnturnedItem(jar.item));

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
        }
    }
}
