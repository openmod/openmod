using OpenMod.Unturned.Items;
using OpenMod.Unturned.Players.Events.Equipment;

namespace OpenMod.Unturned.Players.Events.Clothing
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
