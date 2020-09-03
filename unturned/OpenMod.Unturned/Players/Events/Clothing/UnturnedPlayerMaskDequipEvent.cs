using OpenMod.Unturned.Items;

namespace OpenMod.Unturned.Players.Events.Clothing
{
    public class UnturnedPlayerMaskDequipEvent : UnturnedPlayerClothingDequipEvent
    {
        public override ClothingType Type => ClothingType.Mask;

        public UnturnedPlayerMaskDequipEvent(UnturnedPlayer player, UnturnedItem item) : base(player, item)
        {

        }
    }
}
