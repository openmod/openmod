using OpenMod.Unturned.Events;

namespace OpenMod.Unturned.Players.UI.Events
{
    public abstract class UnturnedPlayerUIEvent : UnturnedPlayerEvent
    {
        protected UnturnedPlayerUIEvent(UnturnedPlayer player) : base(player)
        {
        }
    }
}
