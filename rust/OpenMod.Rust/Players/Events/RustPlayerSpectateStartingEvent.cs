using OpenMod.API.Eventing;

namespace OpenMod.Rust.Players.Events
{
    public class RustPlayerSpectateStartingEvent : RustPlayerEvent, ICancellableEvent
    {
        public string SpectateFilter { get; }
        public bool IsCancelled { get; set; }
     
        public RustPlayerSpectateStartingEvent(
            RustPlayer player,
            string spectateFilter) : base(player)
        {
            SpectateFilter = spectateFilter;
        }
    }
}