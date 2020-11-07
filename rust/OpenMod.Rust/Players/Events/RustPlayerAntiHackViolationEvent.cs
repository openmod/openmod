using OpenMod.API.Eventing;

namespace OpenMod.Rust.Players.Events
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