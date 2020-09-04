using OpenMod.Unturned.Items;

namespace OpenMod.Unturned.Players.Events.Clothing
{
    public class UnturnedPlayerHatUnequippingEvent : UnturnedPlayerClothingUnequippingEvent
    {
        public override ClothingType Type => ClothingType.Hat;

        public UnturnedPlayerHatUnequippingEvent(UnturnedPlayer player, UnturnedItem item) : base(player, item)
        {

        }
    }
}
