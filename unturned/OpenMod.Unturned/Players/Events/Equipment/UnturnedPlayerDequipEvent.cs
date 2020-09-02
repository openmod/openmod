using OpenMod.Extensions.Games.Abstractions.Items;
using OpenMod.Extensions.Games.Abstractions.Players;
using OpenMod.Unturned.Events;
using OpenMod.Unturned.Items;
using SDG.Unturned;

namespace OpenMod.Unturned.Players.Events.Equipment
{
    public abstract class UnturnedPlayerDequipEvent : UnturnedPlayerEvent, IPlayerItemUnequipEvent
    {
        public Item Item { get; }

        IItem IItemEvent.Item => new UnturnedItem(Item);

        public bool IsCancelled { get; set; }

        protected UnturnedPlayerDequipEvent(UnturnedPlayer player, Item item) : base(player)
        {
            Item = item;
        }
    }
}
