using OpenMod.Unturned.Entities;
using OpenMod.Unturned.Events.Players.Equipment;
using SDG.Unturned;

namespace OpenMod.Unturned.Events.Players.Clothing
{
    public abstract class UnturnedPlayerClothingDequipEvent : UnturnedPlayerItemDequipEvent
    {
        protected UnturnedPlayerClothingDequipEvent(UnturnedPlayer player, Item item) : base(player, item)
        {

        }
    }
}
