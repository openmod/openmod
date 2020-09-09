using OpenMod.Unturned.Items;

namespace OpenMod.Unturned.Players.Events.Clothing
{
    public class UnturnedPlayerVestUnequippedEvent : UnturnedPlayerClothingUnequippedEvent
    {
        public override ClothingType Type => ClothingType.Vest;

        public UnturnedPlayerVestUnequippedEvent(UnturnedPlayer player) : base(player)
        {

        }
    }
}
