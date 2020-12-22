using OpenMod.Rust.Players.Events;

namespace OpenMod.Rust.Players.Punishments.Events
{
    public class RustPlayerKickedEvent : RustPlayerEvent
    {
        public string Reason { get; }

        public RustPlayerKickedEvent(
            RustPlayer player, 
            string reason) : base(player)
        {
            Reason = reason;
        }
    }
}