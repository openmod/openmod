using OpenMod.Unturned.Items;

namespace OpenMod.Unturned.Players.Events.Clothing
{
    public class UnturnedPlayerGlassesEquipEvent : UnturnedPlayerClothingEquipEvent
    {
        public override ClothingType Type => ClothingType.Glasses;

        public UnturnedPlayerGlassesEquipEvent(UnturnedPlayer player, UnturnedItem item) : base(player, item)
        {

        }
    }
}
