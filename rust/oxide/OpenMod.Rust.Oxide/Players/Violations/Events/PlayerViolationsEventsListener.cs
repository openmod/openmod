using JetBrains.Annotations;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.Rust.Oxide.Events;
using OpenMod.Rust.Players;
using OpenMod.Rust.Players.Violations.Events;
using Oxide.Core.Plugins;

namespace OpenMod.Rust.Oxide.Players.Violations.Events
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    internal class PlayerViolationsEventsListener : OxideEventsListenerBase
    {
        public PlayerViolationsEventsListener(IEventBus eventBus, IOpenModHost openModHost)
            : base(eventBus, openModHost)
        {
        }

        [HookMethod("OnPlayerViolation")]
        private object? OnPlayerViolation(BasePlayer player, AntiHackType type, float amount)
        {
            var @event = new RustPlayerAntiHackViolationEvent(new RustPlayer(player), type, amount);
            return EmitCancellableReturnsObject(@event);
        }

        [HookMethod("OnPlayerReported")]
        private void OnPlayerReported(
            BasePlayer reporter, string targetName, string targetId, string subject, string message, string type)
        {
            var @event = new RustPlayerReportedEvent(
                new RustPlayer(reporter), targetName, targetId, subject, message, type);

            Emit(@event);
        }
    }
}
