using OpenMod.Unturned.Items;
using OpenMod.Unturned.Players.Equipment.Events;

namespace OpenMod.Unturned.Players.Clothing.Events
{
    public class UnturnedPlayerClothingEquippedEvent : UnturnedPlayerItemEquippedEvent
    {
        public ClothingType Type { get; }

        public UnturnedPlayerClothingEquippedEvent(UnturnedPlayer player, UnturnedItem item, ClothingType type) : base(player, item)
        {
            Type = type;
        }
    }
}
