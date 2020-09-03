using OpenMod.Unturned.Items;

namespace OpenMod.Unturned.Players.Events.Clothing
{
    public class UnturnedPlayerPantsEquipEvent : UnturnedPlayerClothingEquipEvent
    {
        public override ClothingType Type => ClothingType.Pants;

        public UnturnedPlayerPantsEquipEvent(UnturnedPlayer player, UnturnedItem item) : base(player, item)
        {

        }
    }
}
