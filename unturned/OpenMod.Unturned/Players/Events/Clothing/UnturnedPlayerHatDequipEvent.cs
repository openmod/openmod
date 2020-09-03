using SDG.Unturned;

namespace OpenMod.Unturned.Players.Events.Clothing
{
    public class UnturnedPlayerHatDequipEvent : UnturnedPlayerClothingDequipEvent
    {
        public override ClothingType Type => ClothingType.Hat;

        public UnturnedPlayerHatDequipEvent(UnturnedPlayer player, Item item) : base(player, item)
        {

        }
    }
}
