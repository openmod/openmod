using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Users;
using OpenMod.Unturned.Entities;
using SDG.Unturned;

namespace OpenMod.Unturned.Events.Players.Equipment
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

            UnturnedPlayerItemEquipEvent @event = new UnturnedPlayerItemEquipEvent(player, jar, asset);

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
        }

        private void OnDequipRequested(PlayerEquipment equipment, ref bool shouldAllow)
        {
            UnturnedPlayer player = GetUnturnedPlayer(equipment.player);

            UnturnedPlayerItemDequipEvent @event = new UnturnedPlayerItemDequipEvent(player);

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
        }
    }
}
