using OpenMod.Extensions.Games.Abstractions.Players;
using OpenMod.Rust.Players.Events;

namespace OpenMod.Rust.Players.Life.Events
{
    public class RustPlayerSpawnedEvent : RustPlayerEvent, IPlayerSpawnedEvent
    {
        public RustPlayerSpawnedEvent(RustPlayer player) : base(player)
        {

        }
    }
}