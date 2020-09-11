using OpenMod.Extensions.Games.Abstractions.Items;
using OpenMod.Extensions.Games.Abstractions.Players;
using OpenMod.Unturned.Events;
using OpenMod.Unturned.Items;

namespace OpenMod.Unturned.Players.Equipment.Events
{
    public class UnturnedPlayerItemEquippedEvent : UnturnedPlayerEvent, IPlayerItemEquippedEvent
    {
        public UnturnedItem Item { get; }

        IItem IItemEvent.Item => Item;

        public UnturnedPlayerItemEquippedEvent(UnturnedPlayer player, UnturnedItem item) : base(player)
        {
            Item = item;
        }
    }
}