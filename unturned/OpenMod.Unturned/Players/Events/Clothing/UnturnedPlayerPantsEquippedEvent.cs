using OpenMod.Unturned.Items;

namespace OpenMod.Unturned.Players.Events.Clothing
{
    public class UnturnedPlayerPantsEquippedEvent : UnturnedPlayerClothingEquippedEvent
    {
        public override ClothingType Type => ClothingType.Pants;

        public UnturnedPlayerPantsEquippedEvent(UnturnedPlayer player, UnturnedItem item) : base(player, item)
        {

        }
    }
}
