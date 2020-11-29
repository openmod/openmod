using OpenMod.API.Eventing;
using OpenMod.Rust.Players.Events;

namespace OpenMod.Rust.Players.Inventory.Events
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