using JetBrains.Annotations;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.Rust.Oxide.Events;
using OpenMod.Rust.Players;
using OpenMod.Rust.Players.Life.Events;
using Oxide.Core.Plugins;

namespace OpenMod.Rust.Oxide.Players.Life.Events
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    internal class PlayerLifeEventsListener : OxideEventsListenerBase
    {
        public PlayerLifeEventsListener(IEventBus eventBus, IOpenModHost openModHost) : base(eventBus, openModHost)
        {
        }

        [HookMethod("OnPlayerAssist")]
        private object? OnPlayerAssist(BasePlayer target, BasePlayer player)
        {
            var @event = new RustPlayerAssistingEvent(new RustPlayer(player), new RustPlayer(target));
            return EmitCancellableReturnsObject(@event);
        }

        [HookMethod("OnPlayerAttack")]
        private object? OnPlayerAttack(BasePlayer attacker, HitInfo info)
        {
            var @event = new RustPlayerAttackingEvent(new RustPlayer(attacker), info);
            return EmitCancellableReturnsObject(@event);
        }

        [HookMethod("OnPlayerDeath")]
        private object? OnPlayerDeath(BasePlayer player, HitInfo info)
        {
            var @event = new RustPlayerDeathEvent(new RustPlayer(player), info);
            Emit(@event);
            return null;
        }

        [HookMethod("OnPlayerWound")]
        private object? OnPlayerWound(BasePlayer player, BasePlayer source)
        {
            var @event = new RustPlayerGettingWoundedEvent(
                new RustPlayer(player),
                source == null ? null : new RustPlayer(source));

            return EmitCancellableReturnsObject(@event);
        }

        [HookMethod("OnPlayerHealthChange")]
        private object? OnPlayerHealthChange(BasePlayer player, float oldValue, float newValue)
        {
            var @event = new RustPlayerHealthUpdatingEvent(new RustPlayer(player), oldValue, newValue);
            return EmitCancellableReturnsObject(@event);
        }

        [HookMethod("OnMeleeAttack")]
        private object? OnMeleeAttack(BasePlayer player, HitInfo info)
        {
            var @event = new RustPlayerMeleeAttackingEvent(new RustPlayer(player), info);
            return EmitCancellableReturnsObject(@event);
        }

        [HookMethod("OnPlayerRecovered")]
        private void OnPlayerRecovered(BasePlayer player)
        {
            var @event = new RustPlayerRecoveredEvent(new RustPlayer(player));
            Emit(@event);
        }

        [HookMethod("OnPlayerRecover")]
        private object? OnPlayerRecover(BasePlayer player)
        {
            var @event = new RustPlayerRecoveringEvent(new RustPlayer(player));
            return EmitCancellableReturnsObject(@event);
        }

        [HookMethod("OnPlayerRespawned")]
        private void OnPlayerRespawned(BasePlayer player)
        {
            var @event = new RustPlayerRespawnedEvent(new RustPlayer(player));
            Emit(@event);
        }

        [HookMethod("OnPlayerRespawn")]
        private object? OnPlayerRespawn(BasePlayer player)
        {
            var @event = new RustPlayerRespawningEvent(new RustPlayer(player));
            return EmitCancellableReturnsObject(@event);
        }

        [HookMethod("OnPlayerRevive")]
        private object? OnPlayerRevive(BasePlayer reviver, BasePlayer player)
        {
            var @event = new RustPlayerRevivingEvent(new RustPlayer(player), new RustPlayer(player));
            return EmitCancellableReturnsObject(@event);
        }

        [HookMethod("OnPlayerSleepEnded")]
        private void OnPlayerSleepEnded(BasePlayer player)
        {
            var @event = new RustPlayerSleepEndedEvent(new RustPlayer(player));
            Emit(@event);
        }

        [HookMethod("OnPlayerSleep")]
        private object? OnPlayerSleep(BasePlayer player)
        {
            var @event = new RustPlayerSleepStartingEvent(new RustPlayer(player));
            return EmitCancellableReturnsObject(@event);
        }
    }
}
