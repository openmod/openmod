using OpenMod.API.Eventing;
using OpenMod.Rust.Players.Events;

namespace OpenMod.Rust.Players.Entities.Events
{
    public class RustPlayerAssigningSleepingBagEvent : RustPlayerEvent, ICancellableEvent
    {
        public SleepingBag SleepingBag { get; }
        public ulong TargetPlayerId { get; }
        public bool IsCancelled { get; set; }

        public RustPlayerAssigningSleepingBagEvent(
            RustPlayer player,
            SleepingBag sleepingBag,
            ulong targetPlayerId) : base(player)
        {
            SleepingBag = sleepingBag;
            TargetPlayerId = targetPlayerId;
        }
    }
}