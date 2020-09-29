using OpenMod.API.Eventing;
using OpenMod.Rust.Players.Events;

namespace OpenMod.Rust.Players.Entities.Events
{
    public class RustPlayerChangingLockCodeEvent : RustPlayerEvent, ICancellableEvent
    {
        public CodeLock CodeLock { get; }
        public string NewCode { get; }
        public bool IsGuestCode { get; }

        public RustPlayerChangingLockCodeEvent(RustPlayer player,
            CodeLock codeLock, 
            string newCode, 
            bool isGuestCode) : base(player)
        {
            CodeLock = codeLock;
            NewCode = newCode;
            IsGuestCode = isGuestCode;
        }

        public bool IsCancelled { get; set; }
    }
}