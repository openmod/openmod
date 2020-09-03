using SDG.Unturned;

namespace OpenMod.Unturned.Players.Events.Clothing
{
    public class UnturnedPlayerBackpackEquipEvent : UnturnedPlayerClothingEquipEvent
    {
        public override ClothingType Type => ClothingType.Backpack;

        public UnturnedPlayerBackpackEquipEvent(UnturnedPlayer player, Item item) : base(player, item)
        {

        }
    }
}
