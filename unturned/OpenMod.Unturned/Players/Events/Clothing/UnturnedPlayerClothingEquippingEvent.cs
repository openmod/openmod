using OpenMod.Unturned.Items;
using OpenMod.Unturned.Players.Events.Equipment;

namespace OpenMod.Unturned.Players.Events.Clothing
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
