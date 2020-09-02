using OpenMod.Unturned.Entities;
using OpenMod.Unturned.Players.Events.Equipment;
using SDG.Unturned;

namespace OpenMod.Unturned.Players.Events.Clothing
{
    public abstract class UnturnedPlayerClothingDequipEvent : UnturnedPlayerItemDequipEvent
    {
        protected UnturnedPlayerClothingDequipEvent(UnturnedPlayer player, Item item) : base(player, item)
        {

        }
    }
}
