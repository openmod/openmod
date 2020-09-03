using OpenMod.Unturned.Items;

namespace OpenMod.Unturned.Players.Events.Clothing
{
    public class UnturnedPlayerVestDequipEvent : UnturnedPlayerClothingDequipEvent
    {
        public override ClothingType Type => ClothingType.Vest;

        public UnturnedPlayerVestDequipEvent(UnturnedPlayer player, UnturnedItem item) : base(player, item)
        {

        }
    }
}
