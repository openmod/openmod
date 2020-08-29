using OpenMod.Unturned.Entities;
using SDG.Unturned;

namespace OpenMod.Unturned.Events.Players.Clothing
{
    public class UnturnedPlayerVestDequipEvent : UnturnedPlayerClothingDequipEvent
    {
        public UnturnedPlayerVestDequipEvent(UnturnedPlayer player, Item item) : base(player, item)
        {

        }
    }
}
