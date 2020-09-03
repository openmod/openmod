using OpenMod.Unturned.Items;

namespace OpenMod.Unturned.Players.Events.Equipment
{
    public class UnturnedPlayerItemDequipEvent : UnturnedPlayerDequipEvent
    {
        public UnturnedPlayerItemDequipEvent(UnturnedPlayer player, UnturnedItem item) : base(player, item)
        {

        }
    }
}
