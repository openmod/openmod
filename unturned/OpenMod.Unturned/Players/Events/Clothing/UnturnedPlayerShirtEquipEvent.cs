using OpenMod.Unturned.Entities;
using SDG.Unturned;

namespace OpenMod.Unturned.Players.Events.Clothing
{
    public class UnturnedPlayerShirtEquipEvent : UnturnedPlayerClothingEquipEvent
    {
        public override ClothingType Type => ClothingType.Shirt;

        public UnturnedPlayerShirtEquipEvent(UnturnedPlayer player, Item item) : base(player, item)
        {

        }
    }
}
