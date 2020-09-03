using OpenMod.Unturned.Items;

namespace OpenMod.Unturned.Players.Events.Clothing
{
    public class UnturnedPlayerHatEquipEvent : UnturnedPlayerClothingEquipEvent
    {
        public override ClothingType Type => ClothingType.Hat;

        public UnturnedPlayerHatEquipEvent(UnturnedPlayer player, UnturnedItem item) : base(player, item)
        {

        }
    }
}
