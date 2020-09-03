using OpenMod.Unturned.Items;

namespace OpenMod.Unturned.Players.Events.Clothing
{
    public class UnturnedPlayerShirtEquipEvent : UnturnedPlayerClothingEquipEvent
    {
        public override ClothingType Type => ClothingType.Shirt;

        public UnturnedPlayerShirtEquipEvent(UnturnedPlayer player, UnturnedItem item) : base(player, item)
        {

        }
    }
}
