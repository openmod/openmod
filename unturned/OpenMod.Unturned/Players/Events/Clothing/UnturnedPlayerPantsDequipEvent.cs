using SDG.Unturned;

namespace OpenMod.Unturned.Players.Events.Clothing
{
    public class UnturnedPlayerPantsDequipEvent : UnturnedPlayerClothingDequipEvent
    {
        public override ClothingType Type => ClothingType.Pants;

        public UnturnedPlayerPantsDequipEvent(UnturnedPlayer player, Item item) : base(player, item)
        {

        }
    }
}
