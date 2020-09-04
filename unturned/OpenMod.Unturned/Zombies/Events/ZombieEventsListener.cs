using HarmonyLib;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Users;
using OpenMod.Unturned.Events;
using OpenMod.Unturned.Players;
using SDG.Unturned;
using System.Linq;
using UnityEngine;

namespace OpenMod.Unturned.Zombies.Events
{
    internal class ZombieEventsListener : UnturnedEventsListener
    {
        public ZombieEventsListener(IOpenModHost openModHost,
            IEventBus eventBus,
            IUserManager userManager) : base(openModHost, eventBus, userManager)
        {

        }

        public override void Subscribe()
        {
            OnZombieAlertingPlayer += EventsOnZombieAlertingPlayer;
            OnZombieAlertingPosition += EventsOnZombieAlertingPosition;
            OnZombieDamaging += EventsOnZombieDamaging;
            OnZombieDead += Events_OnZombieDead;
            OnZombieAdded += Events_OnZombieAdded;
            OnZombieRevived += EventsOnZombieRevived;
        }

        public override void Unsubscribe()
        {
            OnZombieAlertingPlayer -= EventsOnZombieAlertingPlayer;
            OnZombieAlertingPosition -= EventsOnZombieAlertingPosition;
            OnZombieDamaging -= EventsOnZombieDamaging;
            OnZombieDead -= Events_OnZombieDead;
            OnZombieAdded -= Events_OnZombieAdded;
            OnZombieRevived -= EventsOnZombieRevived;
        }

        private void EventsOnZombieAlertingPlayer(Zombie nativeZombie, ref Player nativePlayer, out bool cancel)
        {
            UnturnedZombie zombie = new UnturnedZombie(nativeZombie);

            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedZombieAlertingPlayerEvent @event = new UnturnedZombieAlertingPlayerEvent(zombie, player);

            Emit(@event);

            nativePlayer = @event.Player?.Player;
            cancel = @event.IsCancelled;
        }

        private void EventsOnZombieAlertingPosition(Zombie nativeZombie, ref Vector3 position, ref bool isStartling, out bool cancel)
        {
            UnturnedZombie zombie = new UnturnedZombie(nativeZombie);

            UnturnedZombieAlertingPositionEvent @event = new UnturnedZombieAlertingPositionEvent(zombie, position, isStartling);

            Emit(@event);

            position = @event.Position;
            isStartling = @event.IsStartling;
            cancel = @event.IsCancelled;
        }

        private void EventsOnZombieDamaging(Zombie nativeZombie, ref ushort damageAmount, ref Vector3 ragdoll,
            ref ERagdollEffect ragdollEffect, ref bool trackKill,
            ref bool dropLoot, ref EZombieStunOverride stunOverride, out bool cancel)
        {
            UnturnedZombie zombie = new UnturnedZombie(nativeZombie);

            UnturnedZombieDamagingEvent @event;

            if (damageAmount >= zombie.Health)
            {
                @event = new UnturnedZombieDyingEvent(zombie, damageAmount, ragdoll, ragdollEffect, trackKill, dropLoot,
                    stunOverride);
            }
            else
            {
                @event = new UnturnedZombieDamagingEvent(zombie, damageAmount, ragdoll, ragdollEffect, trackKill,
                    dropLoot, stunOverride);
            }

            Emit(@event);

            damageAmount = @event.DamageAmount;
            ragdoll = @event.Ragdoll;
            ragdollEffect = @event.RagdollEffect;
            trackKill = @event.TrackKill;
            dropLoot = @event.DropLoot;
            stunOverride = @event.StunOverride;
            cancel = @event.IsCancelled;
        }

        private void Events_OnZombieDead(Zombie nativeZombie, Vector3 ragdoll, ERagdollEffect ragdollEffect)
        {
            UnturnedZombie zombie = new UnturnedZombie(nativeZombie);

            UnturnedZombieDeadEvent @event = new UnturnedZombieDeadEvent(zombie, ragdoll, ragdollEffect);

            Emit(@event);
        }

        private void Events_OnZombieAdded(Zombie nativeZombie)
        {
            UnturnedZombie zombie = new UnturnedZombie(nativeZombie);

            UnturnedZombieAddedEvent @event = new UnturnedZombieAddedEvent(zombie);

            Emit(@event);
        }

        private void EventsOnZombieRevived(Zombie nativeZombie)
        {
            UnturnedZombie zombie = new UnturnedZombie(nativeZombie);

            UnturnedZombieRevivedEvent @event = new UnturnedZombieRevivedEvent(zombie);

            Emit(@event);
        }

        private delegate void ZombieAlertingPlayer(Zombie nativeZombie, ref Player player, out bool cancel);
        private static event ZombieAlertingPlayer OnZombieAlertingPlayer;

        private delegate void ZombieAlertingPosition(Zombie nativeZombie, ref Vector3 position, ref bool isStartling,
            out bool cancel);
        private static event ZombieAlertingPosition OnZombieAlertingPosition;

        private delegate void ZombieDamaging(Zombie nativeZombie, ref ushort damageAmount, ref Vector3 ragdoll,
            ref ERagdollEffect ragdollEffect,
            ref bool trackKill, ref bool dropLoot, ref EZombieStunOverride stunOverride, out bool cancel);
        private static event ZombieDamaging OnZombieDamaging;

        private delegate void ZombieSpawned(Zombie nativeZombie);
        private static event ZombieSpawned OnZombieAdded;
        private static event ZombieSpawned OnZombieRevived;

        private delegate void ZombieDead(Zombie nativeZombie, Vector3 ragdoll, ERagdollEffect ragdollEffect);
        private static event ZombieDead OnZombieDead;

        [HarmonyPatch]
        private class ZombiePatches
        {
            [HarmonyPatch(typeof(Zombie), "alert", typeof(Player))]
            [HarmonyPrefix]
            private static bool AlertPlayer(Zombie __instance, ref Player newPlayer, Player ___player)
            {
                if (__instance.isDead || newPlayer == ___player) return true;

                bool cancel = false;

                OnZombieAlertingPlayer?.Invoke(__instance, ref newPlayer, out cancel);

                return !cancel;
            }

            [HarmonyPatch(typeof(Zombie), "alert", typeof(Vector3), typeof(bool))]
            [HarmonyPrefix]
            private static bool AlertPosition(Zombie __instance, ref Vector3 newPosition, ref bool isStartling,
                Player ___player)
            {
                if (__instance.isDead || ___player != null) return true;

                bool cancel = false;

                OnZombieAlertingPosition?.Invoke(__instance, ref newPosition, ref isStartling, out cancel);

                return !cancel;
            }

            [HarmonyPatch(typeof(Zombie), "askDamage")]
            [HarmonyPrefix]
            private static bool AskDamage(Zombie __instance, ref ushort amount, ref Vector3 newRagdoll,
                ref ERagdollEffect ragdollEffect, ref bool trackKill, ref bool dropLoot,
                ref EZombieStunOverride stunOverride)
            {
                if (amount == 0 || __instance.isDead) return true;

                bool cancel = false;

                OnZombieDamaging?.Invoke(__instance, ref amount, ref newRagdoll, ref ragdollEffect,
                    ref trackKill, ref dropLoot, ref stunOverride, out cancel);

                return !cancel;
            }

            [HarmonyPatch(typeof(Zombie), "tellDead")]
            [HarmonyPostfix]
            private static void TellDead(Zombie __instance, Vector3 newRagdoll, ERagdollEffect ragdollEffect)
            {
                OnZombieDead?.Invoke(__instance, newRagdoll, ragdollEffect);
            }

            [HarmonyPatch(typeof(Zombie), "tellAlive")]
            [HarmonyPostfix]
            private static void TellAlive(Zombie __instance)
            {
                OnZombieRevived?.Invoke(__instance);
            }

            [HarmonyPatch(typeof(ZombieManager), "addZombie")]
            [HarmonyPostfix]
            private static void AddZombie(byte bound)
            {
                Zombie zombie = ZombieManager.regions[bound].zombies.LastOrDefault();

                if (zombie != null)
                {
                    OnZombieAdded?.Invoke(zombie);
                }
            }
        }
    }
}
