using OpenMod.Unturned.Players.Events.Equipment;

namespace OpenMod.Unturned.Players.Events.Clothing
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
