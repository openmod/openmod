using OpenMod.Unturned.Items;
using OpenMod.Unturned.Players.Events.Equipment;

namespace OpenMod.Unturned.Players.Events.Clothing
{
    public abstract class UnturnedPlayerClothingDequipEvent : UnturnedPlayerItemDequipEvent
    {
        public abstract ClothingType Type { get; }

        protected UnturnedPlayerClothingDequipEvent(UnturnedPlayer player, UnturnedItem item) : base(player, item)
        {

        }
    }
}
