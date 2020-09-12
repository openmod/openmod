using OpenMod.Unturned.Items;
using OpenMod.Unturned.Players.Equipment.Events;

namespace OpenMod.Unturned.Players.Clothing.Events
{
    public class UnturnedPlayerClothingEquippingEvent : UnturnedPlayerItemEquippingEvent
    {
        public ClothingType Type { get; }

        public UnturnedPlayerClothingEquippingEvent(UnturnedPlayer player, UnturnedItem item, ClothingType type) : base(player, item)
        {
            Type = type;
        }
    }
}
