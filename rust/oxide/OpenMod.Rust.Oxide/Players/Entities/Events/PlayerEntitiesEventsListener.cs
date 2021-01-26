using JetBrains.Annotations;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.Rust.Entities;
using OpenMod.Rust.Oxide.Events;
using OpenMod.Rust.Players;
using OpenMod.Rust.Players.Entities.Events;
using Oxide.Core.Plugins;

namespace OpenMod.Rust.Oxide.Players.Entities.Events
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    internal class PlayerEntitiesEventsListener : OxideEventsListenerBase
    {
        public PlayerEntitiesEventsListener(IEventBus eventBus, IOpenModHost openModHost) : base(eventBus, openModHost)
        {
        }

        [HookMethod("CanUseLockedEntity")]
        private bool CanUseLockedEntity(BasePlayer player, BaseLock @lock)
        {
            var @event = new RustPlayerAccessingLockedEntityEvent(new RustPlayer(player), @lock);
            return EmitCancellableReturnsBool(@event);
        }

        [HookMethod("CanUseMailbox")]
        private bool? CanUseMailbox(BasePlayer player, Mailbox mailbox)
        {
            var @event = new RustPlayerAccessingMailboxEvent(new RustPlayer(player), mailbox);
            return EmitCancellableReturnsBool(@event);
        }

        [HookMethod("CanAssignBed")]
        private object? CanAssignBed(BasePlayer player, SleepingBag bag, ulong targetPlayerId)
        {
            var @event = new RustPlayerAssigningSleepingBagEvent(new RustPlayer(player), bag, targetPlayerId);
            return EmitCancellableReturnsObject(@event);
        }

        [HookMethod("CanChangeCode")]
        private object? CanChangeCode(BasePlayer player, CodeLock codeLock, string newCode, bool isGuestCode)
        {
            var @event = new RustPlayerChangingLockCodeEvent(new RustPlayer(player), codeLock, newCode, isGuestCode);
            return EmitCancellableReturnsObject(@event);
        }

        [HookMethod("OnPlayerCorpseSpawned")]
        private void OnPlayerCorpseSpawned(BasePlayer player, BaseCorpse corpse)
        {
            var @event = new RustPlayerCorpseSpawnedEvent(new RustPlayer(player), corpse);
            Emit(@event);
        }

        [HookMethod("CanDeployItem")]
        private object? CanDeployItem(BasePlayer player, Deployer deployer, uint entityId)
        {
            var @event = new RustPlayerDeployingEntityEvent(new RustPlayer(player), deployer, entityId);
            return EmitCancellableReturnsObject(@event);
        }

        [HookMethod("CanDismountEntity")]
        private object? CanDismountEntity(BasePlayer player, BaseMountable entity)
        {
            var @event = new RustPlayerDismountingEntityEvent(new RustPlayer(player), entity);
            return EmitCancellableReturnsObject(@event);
        }

        [HookMethod("CanHackCrate")]
        private object? CanHackCrate(BasePlayer player, HackableLockedCrate crate)
        {
            var @event = new RustPlayerHackingCrateEvent(new RustPlayer(player), crate);
            return EmitCancellableReturnsObject(@event);
        }

        [HookMethod("CanHideStash")]
        private object? CanHideStash(BasePlayer player, StashContainer stash)
        {
            var @event = new RustPlayerHidingStashEvent(new RustPlayer(player), stash);
            return EmitCancellableReturnsObject(@event);
        }

        [HookMethod("CanLock")]
        private object? CanLock(BasePlayer player, BaseLock @lock)
        {
            var @event = new RustPlayerLockingLockEvent(new RustPlayer(player), @lock);
            return EmitCancellableReturnsObject(@event);
        }

        [HookMethod("OnLootItem")]
        private void OnLootItem(BasePlayer player, Item item)
        {
            var @event = new RustPlayerLootedItemEvent(new RustPlayer(player), item);
            Emit(@event);
        }

        [HookMethod("CanLootEntity")]
        private object? CanLootEntity(BasePlayer player, BaseEntity entity)
        {
            var @event = new RustPlayerLootingEntityEvent(new RustPlayer(player), new RustEntity(entity));
            return EmitCancellableReturnsObject(@event);
        }

        [HookMethod("CanLootPlayer")]
        private bool CanLootPlayer(BasePlayer target, BasePlayer looter)
        {
            var @event = new RustPlayerLootingPlayerEvent(new RustPlayer(looter), new RustPlayer(target));
            return EmitCancellableReturnsBool(@event);
        }

        [HookMethod("CanMountEntity")]
        private object? CanMountEntity(BasePlayer player, BaseMountable entity)
        {
            var @event = new RustPlayerMountingEntityEvent(new RustPlayer(player), entity);
            return EmitCancellableReturnsObject(@event);
        }

        [HookMethod("CanPickupEntity")]
        private bool CanPickupEntity(BasePlayer player, BaseEntity entity)
        {
            var @event = new RustPlayerPickingUpEntityEvent(new RustPlayer(player), new RustEntity(entity));
            return EmitCancellableReturnsBool(@event);
        }

        [HookMethod("CanRenameBed")]
        private object? CanRenameBed(BasePlayer player, SleepingBag bed, string bedName)
        {
            var @event = new RustPlayerRenamingSleepingBagEvent(new RustPlayer(player), bed, bedName);
            return EmitCancellableReturnsObject(@event);
        }

        [HookMethod("CanSeeStash")]
        private object? CanSeeStash(BasePlayer player, StashContainer stash)
        {
            var @event = new RustPlayerRevealingStashEvent(new RustPlayer(player), stash);
            return EmitCancellableReturnsObject(@event);
        }

        [HookMethod("CanUnlock")]
        private object? CanUnlock(BasePlayer player, BaseLock @lock)
        {
            var @event = new RustPlayerUnlockingLockEvent(new RustPlayer(player), @lock);
            return EmitCancellableReturnsObject(@event);
        }

        [HookMethod("CanUpdateSign")]
        private bool CanUpdateSign(BasePlayer player, Signage sign)
        {
            var @event = new RustPlayerUpdatingSignEvent(new RustPlayer(player), sign);
            return EmitCancellableReturnsBool(@event);
        }
    }
}
