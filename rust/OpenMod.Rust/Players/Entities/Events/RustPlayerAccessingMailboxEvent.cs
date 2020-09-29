using OpenMod.API.Eventing;
using OpenMod.Rust.Players.Events;

namespace OpenMod.Rust.Players.Entities.Events
{
    public class RustPlayerAccessingMailboxEvent : RustPlayerEvent, ICancellableEvent
    {
        public Mailbox Mailbox { get; }
        
        public bool IsCancelled { get; set; }
     
        public RustPlayerAccessingMailboxEvent(RustPlayer player, Mailbox mailbox) : base(player)
        {
            Mailbox = mailbox;
        }
    }
}