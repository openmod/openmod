using OpenMod.API.Eventing;

namespace OpenMod.Rust.Players.Events
{
    public class RustPlayerExperimentingEvent: RustPlayerEvent, ICancellableEvent
    {
        public Workbench Workbench { get; }
        public bool IsCancelled { get; set; }

        public RustPlayerExperimentingEvent(
            RustPlayer player,
            Workbench workbench) : base(player)
        {
            Workbench = workbench;
        }
    }
}