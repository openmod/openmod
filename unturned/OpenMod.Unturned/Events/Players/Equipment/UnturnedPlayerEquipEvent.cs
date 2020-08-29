using OpenMod.Extensions.Games.Abstractions.Items;
using OpenMod.Extensions.Games.Abstractions.Players;
using OpenMod.Unturned.Entities;
using OpenMod.Unturned.Items;
using SDG.Unturned;

namespace OpenMod.Unturned.Events.Players.Equipment
{
    public abstract class UnturnedPlayerEquipEvent : UnturnedPlayerEvent, IPlayerItemEquipEvent
    {
        public Item Item { get; }

        IItem IItemEvent.Item => new UnturnedItem(Item);

        public bool IsCancelled { get; set; }

        protected UnturnedPlayerEquipEvent(UnturnedPlayer player, Item item) : base(player)
        {
            Item = item;
        }
    }
}
