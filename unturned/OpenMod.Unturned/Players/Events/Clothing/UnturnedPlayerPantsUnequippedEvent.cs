using OpenMod.Unturned.Items;

namespace OpenMod.Unturned.Players.Events.Clothing
{
    public class UnturnedPlayerPantsUnequippedEvent : UnturnedPlayerClothingUnequippedEvent
    {
        public override ClothingType Type => ClothingType.Pants;

        public UnturnedPlayerPantsUnequippedEvent(UnturnedPlayer player) : base(player)
        {

        }
    }
}
