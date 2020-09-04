using OpenMod.Unturned.Items;

namespace OpenMod.Unturned.Players.Events.Clothing
{
    public class UnturnedPlayerBackpackUnequippingEvent : UnturnedPlayerClothingUnequippingEvent
    {
        public override ClothingType Type => ClothingType.Backpack;

        public UnturnedPlayerBackpackUnequippingEvent(UnturnedPlayer player, UnturnedItem item) : base(player, item)
        {

        }
    }
}
