using SDG.Unturned;

namespace OpenMod.Unturned.Players.Events.Clothing
{
    public class UnturnedPlayerVestEquipEvent : UnturnedPlayerClothingEquipEvent
    {
        public override ClothingType Type => ClothingType.Vest;

        public UnturnedPlayerVestEquipEvent(UnturnedPlayer player, Item item) : base(player, item)
        {

        }
    }
}
