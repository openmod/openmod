using OpenMod.Unturned.Items;

namespace OpenMod.Unturned.Players.Events.Clothing
{
    public class UnturnedPlayerPantsUnequippingEvent : UnturnedPlayerClothingUnequippingEvent
    {
        public override ClothingType Type => ClothingType.Pants;

        public UnturnedPlayerPantsUnequippingEvent(UnturnedPlayer player, UnturnedItem item) : base(player, item)
        {

        }
    }
}
