using SDG.Unturned;

namespace OpenMod.Unturned.Players.Events.Clothing
{
    public class UnturnedPlayerPantsEquipEvent : UnturnedPlayerClothingEquipEvent
    {
        public override ClothingType Type => ClothingType.Pants;

        public UnturnedPlayerPantsEquipEvent(UnturnedPlayer player, Item item) : base(player, item)
        {

        }
    }
}
