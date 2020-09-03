using SDG.Unturned;

namespace OpenMod.Unturned.Players.Events.Clothing
{
    public class UnturnedPlayerMaskDequipEvent : UnturnedPlayerClothingDequipEvent
    {
        public override ClothingType Type => ClothingType.Mask;

        public UnturnedPlayerMaskDequipEvent(UnturnedPlayer player, Item item) : base(player, item)
        {

        }
    }
}
