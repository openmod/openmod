using OpenMod.Unturned.Items;

namespace OpenMod.Unturned.Players.Events.Clothing
{
    public class UnturnedPlayerShirtEquippedEvent : UnturnedPlayerClothingEquippedEvent
    {
        public override ClothingType Type => ClothingType.Shirt;

        public UnturnedPlayerShirtEquippedEvent(UnturnedPlayer player, UnturnedItem item) : base(player, item)
        {

        }
    }
}
