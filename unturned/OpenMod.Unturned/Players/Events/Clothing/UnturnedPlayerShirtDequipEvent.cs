using SDG.Unturned;

namespace OpenMod.Unturned.Players.Events.Clothing
{
    public class UnturnedPlayerShirtDequipEvent : UnturnedPlayerClothingDequipEvent
    {
        public override ClothingType Type => ClothingType.Shirt;

        public UnturnedPlayerShirtDequipEvent(UnturnedPlayer player, Item item) : base(player, item)
        {

        }
    }
}
