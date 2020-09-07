using OpenMod.Unturned.Items;

namespace OpenMod.Unturned.Players.Events.Clothing
{
    public class UnturnedPlayerMaskEquippedEvent : UnturnedPlayerClothingEquippedEvent
    {
        public override ClothingType Type => ClothingType.Mask;

        public UnturnedPlayerMaskEquippedEvent(UnturnedPlayer player, UnturnedItem item) : base(player, item)
        {

        }
    }
}
