using OpenMod.Unturned.Items;

namespace OpenMod.Unturned.Players.Events.Clothing
{
    public class UnturnedPlayerVestUnequippingEvent : UnturnedPlayerClothingUnequippingEvent
    {
        public override ClothingType Type => ClothingType.Vest;

        public UnturnedPlayerVestUnequippingEvent(UnturnedPlayer player, UnturnedItem item) : base(player, item)
        {

        }
    }
}
