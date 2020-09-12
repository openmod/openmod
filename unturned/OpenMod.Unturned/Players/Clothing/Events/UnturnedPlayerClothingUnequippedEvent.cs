using OpenMod.Unturned.Players.Equipment.Events;

namespace OpenMod.Unturned.Players.Clothing.Events
{
    public class UnturnedPlayerClothingUnequippedEvent : UnturnedPlayerItemUnequippedEvent
    {
        public ClothingType Type { get; }

        public UnturnedPlayerClothingUnequippedEvent(UnturnedPlayer player, ClothingType type) : base(player)
        {
            Type = type;
        }
    }
}
