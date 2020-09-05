using OpenMod.Core.Eventing;
using OpenMod.Extensions.Games.Abstractions.Players;
using OpenMod.Unturned.Players;

namespace OpenMod.Unturned.Events
{
    public abstract class UnturnedPlayerEvent : Event, IPlayerEvent
    {
        protected UnturnedPlayerEvent(UnturnedPlayer player)
        {
            Player = player;
        }

        public UnturnedPlayer Player { get; }

        IPlayer IPlayerEvent.Player => Player;
    }
}
