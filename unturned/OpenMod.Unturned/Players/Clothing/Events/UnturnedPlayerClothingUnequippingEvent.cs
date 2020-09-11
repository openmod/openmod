using OpenMod.Unturned.Items;
using OpenMod.Unturned.Players.Equipment.Events;

namespace OpenMod.Unturned.Players.Clothing.Events
{
    public class UnturnedPlayerClothingUnequippingEvent : UnturnedPlayerItemUnequippingEvent
    {
        public ClothingType Type { get; }

        public UnturnedPlayerClothingUnequippingEvent(UnturnedPlayer player, UnturnedItem item, ClothingType type) : base(player, item)
        {
            Type = type;
        }
    }
}
