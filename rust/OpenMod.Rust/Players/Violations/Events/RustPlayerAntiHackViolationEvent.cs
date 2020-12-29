using OpenMod.API.Eventing;
using OpenMod.Rust.Players.Events;

namespace OpenMod.Rust.Players.Violations.Events
{
    public class RustPlayerAntiHackViolationEvent : RustPlayerEvent, ICancellableEvent
    {
        public AntiHackType AntiHackType { get; }
        public float Amount { get; }
        public bool IsCancelled { get; set; }

        public RustPlayerAntiHackViolationEvent(
            RustPlayer player,
            AntiHackType antiHackType,
            float amount) : base(player)
        {
            AntiHackType = antiHackType;
            Amount = amount;
        }
    }
}