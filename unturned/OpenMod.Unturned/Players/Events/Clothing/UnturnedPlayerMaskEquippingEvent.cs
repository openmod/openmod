using OpenMod.Unturned.Items;

namespace OpenMod.Unturned.Players.Events.Clothing
{
    public class UnturnedPlayerMaskEquippingEvent : UnturnedPlayerClothingEquippingEvent
    {
        public override ClothingType Type => ClothingType.Mask;

        public UnturnedPlayerMaskEquippingEvent(UnturnedPlayer player, UnturnedItem item) : base(player, item)
        {

        }
    }
}
