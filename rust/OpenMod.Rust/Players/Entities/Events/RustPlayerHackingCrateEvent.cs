using OpenMod.API.Eventing;
using OpenMod.Rust.Players.Events;

namespace OpenMod.Rust.Players.Entities.Events
{
    public class RustPlayerHackingCrateEvent : RustPlayerEvent, ICancellableEvent
    {
        public HackableLockedCrate Crate { get; }
        public bool IsCancelled { get; set; }
     
        public RustPlayerHackingCrateEvent(
            RustPlayer player,
            HackableLockedCrate crate) : base(player)
        {
            Crate = crate;
        }
    }
}