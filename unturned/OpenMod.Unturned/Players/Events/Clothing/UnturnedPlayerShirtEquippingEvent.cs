using OpenMod.Unturned.Items;

namespace OpenMod.Unturned.Players.Events.Clothing
{
    public class UnturnedPlayerShirtEquippingEvent : UnturnedPlayerClothingEquippingEvent
    {
        public override ClothingType Type => ClothingType.Shirt;

        public UnturnedPlayerShirtEquippingEvent(UnturnedPlayer player, UnturnedItem item) : base(player, item)
        {

        }
    }
}
