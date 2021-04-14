using OpenMod.Unturned.Items;
using OpenMod.Unturned.Players.Equipment.Events;

namespace OpenMod.Unturned.Players.Clothing.Events
{
    public class UnturnedPlayerClothingEquippingEvent : UnturnedPlayerItemEquippingEvent
    {
        public ClothingType Type { get; }

        public UnturnedPlayerClothingEquippingEvent(UnturnedPlayer player, UnturnedClothingItem item) : base(player, item)
        {
            Type = item.ClothingType;
        }
    }
}
