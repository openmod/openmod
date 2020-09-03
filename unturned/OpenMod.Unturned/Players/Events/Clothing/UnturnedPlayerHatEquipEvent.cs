using SDG.Unturned;

namespace OpenMod.Unturned.Players.Events.Clothing
{
    public class UnturnedPlayerHatEquipEvent : UnturnedPlayerClothingEquipEvent
    {
        public override ClothingType Type => ClothingType.Hat;

        public UnturnedPlayerHatEquipEvent(UnturnedPlayer player, Item item) : base(player, item)
        {

        }
    }
}
