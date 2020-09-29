using OpenMod.API.Eventing;
using OpenMod.Rust.Players.Events;

namespace OpenMod.Rust.Players.Entities.Events
{
    public class RustPlayerHidingStashEvent : RustPlayerEvent, ICancellableEvent
    {
        public StashContainer Stash { get; }
        public bool IsCancelled { get; set; }
        
        public RustPlayerHidingStashEvent(
            RustPlayer player,
            StashContainer stash) : base(player)
        {
            Stash = stash;
        }
    }
}