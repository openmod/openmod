using OpenMod.Unturned.Entities;
using SDG.Unturned;

namespace OpenMod.Unturned.Players.Events.Equipment
{
    public class UnturnedPlayerItemDequipEvent : UnturnedPlayerDequipEvent
    {
        public UnturnedPlayerItemDequipEvent(UnturnedPlayer player, Item item) : base(player, item)
        {

        }
    }
}
