using OpenMod.Unturned.Items;

namespace OpenMod.Unturned.Players.Events.Clothing
{
    public class UnturnedPlayerMaskUnequippedEvent : UnturnedPlayerClothingUnequippedEvent
    {
        public override ClothingType Type => ClothingType.Mask;

        public UnturnedPlayerMaskUnequippedEvent(UnturnedPlayer player) : base(player)
        {

        }
    }
}
