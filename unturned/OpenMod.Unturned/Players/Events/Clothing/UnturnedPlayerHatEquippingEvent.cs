using OpenMod.Unturned.Items;

namespace OpenMod.Unturned.Players.Events.Clothing
{
    public class UnturnedPlayerHatEquippingEvent : UnturnedPlayerClothingEquippingEvent
    {
        public override ClothingType Type => ClothingType.Hat;

        public UnturnedPlayerHatEquippingEvent(UnturnedPlayer player, UnturnedItem item) : base(player, item)
        {

        }
    }
}
