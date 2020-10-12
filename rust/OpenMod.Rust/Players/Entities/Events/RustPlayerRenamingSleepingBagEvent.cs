using OpenMod.API.Eventing;
using OpenMod.Rust.Players.Events;

namespace OpenMod.Rust.Players.Entities.Events
{
    public class RustPlayerRenamingSleepingBagEvent : RustPlayerEvent, ICancellableEvent
    {
        public SleepingBag SleepingBag { get; }
        public string NewName { get; }
        public bool IsCancelled { get; set; }

        public RustPlayerRenamingSleepingBagEvent(
            RustPlayer player,
            SleepingBag sleepingBag, 
            string newName) : base(player)
        {
            SleepingBag = sleepingBag;
            NewName = newName;
        }
    }
}