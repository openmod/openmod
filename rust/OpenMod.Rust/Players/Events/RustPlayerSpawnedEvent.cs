using OpenMod.Extensions.Games.Abstractions.Players;

namespace OpenMod.Rust.Players.Events
{
    public class RustPlayerSpawnedEvent : RustPlayerEvent, IPlayerSpawnedEvent
    {
        public RustPlayerSpawnedEvent(RustPlayer player) : base(player)
        {

        }
    }
}