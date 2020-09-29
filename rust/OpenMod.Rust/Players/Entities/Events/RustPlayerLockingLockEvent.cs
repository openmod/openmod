using OpenMod.API.Eventing;
using OpenMod.Rust.Players.Events;

namespace OpenMod.Rust.Players.Entities.Events
{
    public class RustPlayerLockingLockEvent : RustPlayerEvent, ICancellableEvent
    {
        public BaseLock Lock { get; }

        public bool IsCancelled { get; set; }

        public RustPlayerLockingLockEvent(
            RustPlayer player,
            BaseLock @lock) : base(player)
        {
            Lock = @lock;
        }
    }
}