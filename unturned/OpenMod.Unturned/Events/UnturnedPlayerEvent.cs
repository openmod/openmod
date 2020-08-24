using OpenMod.Core.Eventing;
using OpenMod.Unturned.Entities;

namespace OpenMod.Unturned.Events
{
    public abstract class UnturnedPlayerEvent : Event
    {
        protected UnturnedPlayerEvent(UnturnedPlayer player)
        {
            Player = player;
        }

        public UnturnedPlayer Player { get; }
    }
}
