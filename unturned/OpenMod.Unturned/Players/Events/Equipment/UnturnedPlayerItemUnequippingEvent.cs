using OpenMod.Unturned.Items;

namespace OpenMod.Unturned.Players.Events.Equipment
{
    public class UnturnedPlayerItemUnequippingEvent : UnturnedPlayerUnequippingEvent
    {
        public UnturnedPlayerItemUnequippingEvent(UnturnedPlayer player, UnturnedItem item) : base(player, item)
        {

        }
    }
}
