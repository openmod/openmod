using OpenMod.Unturned.Items;

namespace OpenMod.Unturned.Players.Events.Clothing
{
    public class UnturnedPlayerVestEquippingEvent : UnturnedPlayerClothingEquippingEvent
    {
        public override ClothingType Type => ClothingType.Vest;

        public UnturnedPlayerVestEquippingEvent(UnturnedPlayer player, UnturnedItem item) : base(player, item)
        {

        }
    }
}
