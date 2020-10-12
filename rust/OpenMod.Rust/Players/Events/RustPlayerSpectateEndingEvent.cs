using OpenMod.API.Eventing;

namespace OpenMod.Rust.Players.Events
{
    public class RustPlayerSpectateEndingEvent : RustPlayerEvent, ICancellableEvent
    {
        public string SpectateFilter { get; }
        public bool IsCancelled { get; set; }
        
        public RustPlayerSpectateEndingEvent(
            RustPlayer player,
            string spectateFilter) : base(player)
        {
            SpectateFilter = spectateFilter;
        }
    }
}