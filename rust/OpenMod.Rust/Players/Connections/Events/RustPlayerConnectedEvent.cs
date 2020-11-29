using OpenMod.Rust.Players.Events;

namespace OpenMod.Rust.Players.Connections.Events
{
    public class RustPlayerConnectedEvent : RustPlayerEvent
    {
        public RustPlayerConnectedEvent(RustPlayer player) : base(player)
        {
        }
    }
}