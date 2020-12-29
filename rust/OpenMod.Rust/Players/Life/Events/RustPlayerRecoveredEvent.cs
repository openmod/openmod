using OpenMod.Rust.Players.Events;

namespace OpenMod.Rust.Players.Life.Events
{
    public class RustPlayerRecoveredEvent : RustPlayerEvent
    {
        public RustPlayerRecoveredEvent(RustPlayer player) : base(player)
        {
        }
    }
}