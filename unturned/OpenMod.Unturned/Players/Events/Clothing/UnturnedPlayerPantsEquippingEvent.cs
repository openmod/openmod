using OpenMod.Unturned.Items;

namespace OpenMod.Unturned.Players.Events.Clothing
{
    public class UnturnedPlayerPantsEquippingEvent : UnturnedPlayerClothingEquippingEvent
    {
        public override ClothingType Type => ClothingType.Pants;

        public UnturnedPlayerPantsEquippingEvent(UnturnedPlayer player, UnturnedItem item) : base(player, item)
        {

        }
    }
}
