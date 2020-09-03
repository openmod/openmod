using OpenMod.Unturned.Items;

namespace OpenMod.Unturned.Players.Events.Clothing
{
    public class UnturnedPlayerShirtDequipEvent : UnturnedPlayerClothingDequipEvent
    {
        public override ClothingType Type => ClothingType.Shirt;

        public UnturnedPlayerShirtDequipEvent(UnturnedPlayer player, UnturnedItem item) : base(player, item)
        {

        }
    }
}
