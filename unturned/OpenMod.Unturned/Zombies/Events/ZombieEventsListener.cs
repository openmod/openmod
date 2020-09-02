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
            OnZombieAlertPlayer += Events_OnZombieAlertPlayer;
            OnZombieAlertPosition += Events_OnZombieAlertPosition;
            OnZombieDamage += Events_OnZombieDamage;
            OnZombieDead += Events_OnZombieDead;
            OnZombieAdded += Events_OnZombieAdded;
            OnZombieRevive += Events_OnZombieRevive;
        }

        public override void Unsubscribe()
        {
            OnZombieAlertPlayer -= Events_OnZombieAlertPlayer;
            OnZombieAlertPosition -= Events_OnZombieAlertPosition;
            OnZombieDamage -= Events_OnZombieDamage;
            OnZombieDead -= Events_OnZombieDead;
            OnZombieAdded -= Events_OnZombieAdded;
            OnZombieRevive -= Events_OnZombieRevive;
        }

        private void Events_OnZombieAlertPlayer(Zombie nativeZombie, ref Player nativePlayer, out bool cancel)
        {
            UnturnedZombie zombie = new UnturnedZombie(nativeZombie);

            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedZombieAlertPlayerEvent @event = new UnturnedZombieAlertPlayerEvent(zombie, player);

            Emit(@event);

            nativePlayer = @event.Player?.Player;
            cancel = @event.IsCancelled;
        }

        private void Events_OnZombieAlertPosition(Zombie nativeZombie, ref Vector3 position, ref bool isStartling, out bool cancel)
        {
            UnturnedZombie zombie = new UnturnedZombie(nativeZombie);

            UnturnedZombieAlertPositionEvent @event = new UnturnedZombieAlertPositionEvent(zombie, position, isStartling);

            Emit(@event);

            position = @event.Position;
            isStartling = @event.IsStartling;
            cancel = @event.IsCancelled;
        }

        private void Events_OnZombieDamage(Zombie nativeZombie, ref ushort damageAmount, ref Vector3 ragdoll,
            ref ERagdollEffect ragdollEffect, ref bool trackKill,
            ref bool dropLoot, ref EZombieStunOverride stunOverride, out bool cancel)
        {
            UnturnedZombie zombie = new UnturnedZombie(nativeZombie);

            UnturnedZombieDamageEvent @event;

            if (damageAmount >= zombie.Health)
            {
                @event = new UnturnedZombieDyingEvent(zombie, damageAmount, ragdoll, ragdollEffect, trackKill, dropLoot,
                    stunOverride);
            }
            else
            {
                @event = new UnturnedZombieDamageEvent(zombie, damageAmount, ragdoll, ragdollEffect, trackKill,
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

        private void Events_OnZombieRevive(Zombie nativeZombie)
        {
            UnturnedZombie zombie = new UnturnedZombie(nativeZombie);

            UnturnedZombieReviveEvent @event = new UnturnedZombieReviveEvent(zombie);

            Emit(@event);
        }

        private delegate void ZombieAlertPlayer(Zombie nativeZombie, ref Player player, out bool cancel);
        private static event ZombieAlertPlayer OnZombieAlertPlayer;

        private delegate void ZombieAlertPosition(Zombie nativeZombie, ref Vector3 position, ref bool isStartling,
            out bool cancel);
        private static event ZombieAlertPosition OnZombieAlertPosition;

        private delegate void ZombieDamage(Zombie nativeZombie, ref ushort damageAmount, ref Vector3 ragdoll,
            ref ERagdollEffect ragdollEffect,
            ref bool trackKill, ref bool dropLoot, ref EZombieStunOverride stunOverride, out bool cancel);
        private static event ZombieDamage OnZombieDamage;

        private delegate void ZombieSpawn(Zombie nativeZombie);
        private static event ZombieSpawn OnZombieAdded;
        private static event ZombieSpawn OnZombieRevive;

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

                OnZombieAlertPlayer?.Invoke(__instance, ref newPlayer, out cancel);

                return !cancel;
            }

            [HarmonyPatch(typeof(Zombie), "alert", typeof(Vector3), typeof(bool))]
            [HarmonyPrefix]
            private static bool AlertPosition(Zombie __instance, ref Vector3 newPosition, ref bool isStartling,
                Player ___player)
            {
                if (__instance.isDead || ___player != null) return true;

                bool cancel = false;

                OnZombieAlertPosition?.Invoke(__instance, ref newPosition, ref isStartling, out cancel);

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

                OnZombieDamage?.Invoke(__instance, ref amount, ref newRagdoll, ref ragdollEffect,
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
                OnZombieRevive?.Invoke(__instance);
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
