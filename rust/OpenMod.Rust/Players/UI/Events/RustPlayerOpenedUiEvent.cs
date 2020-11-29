using OpenMod.Rust.Players.Events;

namespace OpenMod.Rust.Players.UI.Events
{
    public class RustPlayerOpenedUiEvent : RustPlayerEvent
    {
        public string Json { get; }

        public RustPlayerOpenedUiEvent(RustPlayer player, string json) : base(player)
        {
            Json = json;
        }
    }
}