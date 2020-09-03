using OpenMod.Unturned.Items;
using OpenMod.Unturned.Players.Events.Equipment;

namespace OpenMod.Unturned.Players.Events.Clothing
{
    public abstract class UnturnedPlayerClothingEquipEvent : UnturnedPlayerItemEquipEvent
    {
        public abstract ClothingType Type { get; }

        protected UnturnedPlayerClothingEquipEvent(UnturnedPlayer player, UnturnedItem item) : base(player, item)
        {

        }
    }
}
