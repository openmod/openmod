using OpenMod.Unturned.Items;

namespace OpenMod.Unturned.Players.Events.Clothing
{
    public class UnturnedPlayerGlassesUnequippingEvent : UnturnedPlayerClothingUnequippingEvent
    {
        public override ClothingType Type => ClothingType.Glasses;

        public UnturnedPlayerGlassesUnequippingEvent(UnturnedPlayer player, UnturnedItem item) : base(player, item)
        {

        }
    }
}
