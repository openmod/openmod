using OpenMod.Rust.Players.Events;

namespace OpenMod.Rust.Players.UI.Events
{
    public class RustPlayerClosedUiEvent : RustPlayerEvent
    {
        public string Json { get; }

        public RustPlayerClosedUiEvent(RustPlayer player, string json) : base(player)
        {
            Json = json;
        }
    }
}