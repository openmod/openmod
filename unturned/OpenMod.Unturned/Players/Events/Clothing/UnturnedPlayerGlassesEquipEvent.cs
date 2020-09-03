using SDG.Unturned;

namespace OpenMod.Unturned.Players.Events.Clothing
{
    public class UnturnedPlayerGlassesEquipEvent : UnturnedPlayerClothingEquipEvent
    {
        public override ClothingType Type => ClothingType.Glasses;

        public UnturnedPlayerGlassesEquipEvent(UnturnedPlayer player, Item item) : base(player, item)
        {

        }
    }
}
