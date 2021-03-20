extern alias JetBrainsAnnotations;
using System;
using JetBrainsAnnotations::JetBrains.Annotations;
using OpenMod.API.Eventing;
using OpenMod.Unturned.Events;
using OpenMod.Unturned.Items;
using SDG.Unturned;

namespace OpenMod.Unturned.Players.Equipment.Events
{
    [UsedImplicitly]
    internal class PlayerEquipmentEventsListener : UnturnedPlayerEventsListener
    {
        public PlayerEquipmentEventsListener(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override void Subscribe()
        {
            PlayerEquipment.OnUseableChanged_Global += PlayerEquipmentOnOnUseableChanged_Global;
        }

        public override void Unsubscribe()
        {
            PlayerEquipment.OnUseableChanged_Global -= PlayerEquipmentOnOnUseableChanged_Global;
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

        private void PlayerEquipmentOnOnUseableChanged_Global(PlayerEquipment equipment)
        {
            var player = GetUnturnedPlayer(equipment.player)!;
            IEvent @event;
            if (equipment.useable is null)
            {
                @event = new UnturnedPlayerItemUnequippedEvent(player);
            }
            else
            {
                var inventory = equipment.player.inventory;
                var item = inventory.getItem(equipment.equippedPage,
                    inventory.getIndex(equipment.equippedPage, equipment.equipped_x, equipment.equipped_y)).item;

                @event = new UnturnedPlayerItemEquippedEvent(player, new UnturnedItem(item));
            }

            Emit(@event);
        }

        private void OnEquipRequested(PlayerEquipment equipment, ItemJar jar, ItemAsset asset, ref bool shouldAllow)
        {
            var player = GetUnturnedPlayer(equipment.player)!;

            var @event = new UnturnedPlayerItemEquippingEvent(player, new UnturnedItem(jar.item))
            {
                IsCancelled = !shouldAllow
            };

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
        }

        private void OnDequipRequested(PlayerEquipment equipment, ref bool shouldAllow)
        {
            var player = GetUnturnedPlayer(equipment.player)!;

            var inv = player.Player.inventory;

            var page = equipment.equippedPage;

            var index = inv.getIndex(page, equipment.equipped_x, equipment.equipped_y);

            var jar = inv.getItem(page, index);

            if (jar?.item == null)
                return;

            var @event = new UnturnedPlayerItemUnequippingEvent(player, new UnturnedItem(jar.item))
            {
                IsCancelled = !shouldAllow
            };

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
        }
    }
}
