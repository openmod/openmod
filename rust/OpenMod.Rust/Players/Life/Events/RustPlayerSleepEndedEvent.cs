using OpenMod.Rust.Players.Events;

namespace OpenMod.Rust.Players.Life.Events
{
    public class RustPlayerSleepEndedEvent : RustPlayerEvent
    {
        public RustPlayerSleepEndedEvent(RustPlayer player) : base(player)
        {
        }
    }
}
