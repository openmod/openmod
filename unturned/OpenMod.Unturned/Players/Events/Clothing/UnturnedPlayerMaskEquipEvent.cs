using SDG.Unturned;

namespace OpenMod.Unturned.Players.Events.Clothing
{
    public class UnturnedPlayerMaskEquipEvent : UnturnedPlayerClothingEquipEvent
    {
        public override ClothingType Type => ClothingType.Mask;

        public UnturnedPlayerMaskEquipEvent(UnturnedPlayer player, Item item) : base(player, item)
        {

        }
    }
}
