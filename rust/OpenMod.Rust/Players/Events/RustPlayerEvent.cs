using OpenMod.Core.Eventing;
using OpenMod.Extensions.Games.Abstractions.Players;

namespace OpenMod.Rust.Players.Events
{
    public class RustPlayerEvent : Event, IPlayerEvent
    {
        public RustPlayerEvent(RustPlayer player)
        {
            Player = player;
        }

        public RustPlayer Player { get; }

        IPlayer IPlayerEvent.Player => Player;
    }
}