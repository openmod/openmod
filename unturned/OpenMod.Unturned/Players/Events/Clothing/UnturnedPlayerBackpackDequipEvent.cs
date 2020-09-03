using OpenMod.Unturned.Items;

namespace OpenMod.Unturned.Players.Events.Clothing
{
    public class UnturnedPlayerBackpackDequipEvent : UnturnedPlayerClothingDequipEvent
    {
        public override ClothingType Type => ClothingType.Backpack;

        public UnturnedPlayerBackpackDequipEvent(UnturnedPlayer player, UnturnedItem item) : base(player, item)
        {

        }
    }
}
