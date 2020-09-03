using OpenMod.Unturned.Items;

namespace OpenMod.Unturned.Players.Events.Clothing
{
    public class UnturnedPlayerGlassesDequipEvent : UnturnedPlayerClothingDequipEvent
    {
        public override ClothingType Type => ClothingType.Glasses;

        public UnturnedPlayerGlassesDequipEvent(UnturnedPlayer player, UnturnedItem item) : base(player, item)
        {

        }
    }
}
