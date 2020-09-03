using OpenMod.Unturned.Items;

namespace OpenMod.Unturned.Players.Events.Clothing
{
    public class UnturnedPlayerMaskEquipEvent : UnturnedPlayerClothingEquipEvent
    {
        public override ClothingType Type => ClothingType.Mask;

        public UnturnedPlayerMaskEquipEvent(UnturnedPlayer player, UnturnedItem item) : base(player, item)
        {

        }
    }
}
