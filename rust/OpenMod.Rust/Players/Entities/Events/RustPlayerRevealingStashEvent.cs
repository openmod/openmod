using OpenMod.API.Eventing;
using OpenMod.Rust.Players.Events;

namespace OpenMod.Rust.Players.Entities.Events
{
    public class RustPlayerRevealingStashEvent : RustPlayerEvent, ICancellableEvent
    {
        public StashContainer Stash { get; }

        public bool IsCancelled { get; set; }
     
        public RustPlayerRevealingStashEvent(RustPlayer player, StashContainer stash) : base(player)
        {
            Stash = stash;
        }
    }
}