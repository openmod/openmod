using SDG.Unturned;

namespace OpenMod.Unturned.Players.Events.Clothing
{
    public class UnturnedPlayerVestDequipEvent : UnturnedPlayerClothingDequipEvent
    {
        public override ClothingType Type => ClothingType.Vest;

        public UnturnedPlayerVestDequipEvent(UnturnedPlayer player, Item item) : base(player, item)
        {

        }
    }
}
