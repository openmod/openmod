using OpenMod.Unturned.Items;

namespace OpenMod.Unturned.Players.Events.Clothing
{
    public class UnturnedPlayerVestEquippedEvent : UnturnedPlayerClothingEquippedEvent
    {
        public override ClothingType Type => ClothingType.Vest;

        public UnturnedPlayerVestEquippedEvent(UnturnedPlayer player, UnturnedItem item) : base(player, item)
        {

        }
    }
}
