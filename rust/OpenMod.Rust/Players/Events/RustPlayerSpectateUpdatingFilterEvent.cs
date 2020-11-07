using OpenMod.API.Eventing;

namespace OpenMod.Rust.Players.Events
{
    public class RustPlayerSpectateUpdatingFilterEvent : RustPlayerEvent, ICancellableEvent
    {
        public string SpectateFilter { get; }
        public bool IsCancelled { get; set; }

        public RustPlayerSpectateUpdatingFilterEvent(
            RustPlayer player,
            string spectateFilter) : base(player)
        {
            SpectateFilter = spectateFilter;
        }
    }
}