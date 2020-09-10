using OpenMod.Unturned.Items;

namespace OpenMod.Unturned.Players.Events.Equipment
{
    public class UnturnedPlayerItemEquippedEvent : UnturnedPlayerEquippedEvent
    {
        public UnturnedPlayerItemEquippedEvent(UnturnedPlayer player, UnturnedItem item) : base(player, item)
        {

        }
    }
}
