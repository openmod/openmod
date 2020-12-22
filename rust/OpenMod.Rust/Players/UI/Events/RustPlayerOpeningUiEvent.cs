using OpenMod.API.Eventing;
using OpenMod.Rust.Players.Events;

namespace OpenMod.Rust.Players.UI.Events
{
    public class RustPlayerOpeningUiEvent : RustPlayerEvent, ICancellableEvent
    {
        public string Json { get; }
        
        public bool IsCancelled { get; set; }

        public RustPlayerOpeningUiEvent(RustPlayer player, string json) : base(player)
        {
            Json = json;
        }
    }
}
