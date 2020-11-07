using OpenMod.API.Eventing;
using OpenMod.Rust.Players.Events;

namespace OpenMod.Rust.Players.Inventory.Events
{
    public class RustPlayerPickingUpLockEvent : RustPlayerEvent, ICancellableEvent
    {
        public BaseLock Lock { get; }
        public bool IsCancelled { get; set; }
     
        public RustPlayerPickingUpLockEvent(
            RustPlayer player, 
            BaseLock @lock) : base(player)
        {
            Lock = @lock;
        }
    }
}