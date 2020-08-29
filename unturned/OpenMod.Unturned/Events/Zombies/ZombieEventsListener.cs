using HarmonyLib;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Users;
using SDG.Unturned;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace OpenMod.Unturned.Events.Zombies
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
            OnZombieDamage += Events_OnZombieDamage;
            OnZombieDead += Events_OnZombieDead;
            OnZombieSpawn += Events_OnZombieSpawn;
        }

        public override void Unsubscribe()
        {
            OnZombieDamage -= Events_OnZombieDamage;
            OnZombieSpawn -= Events_OnZombieSpawn;
        }

        private readonly FieldInfo ZombieHealth =
            typeof(Zombie).GetField("health", BindingFlags.Instance | BindingFlags.NonPublic);

        private void Events_OnZombieDamage(Zombie zombie, ref ushort damageAmount, ref Vector3 ragdoll,
            ref ERagdollEffect ragdollEffect, ref bool trackKill,
            ref bool dropLoot, ref EZombieStunOverride stunOverride, out bool cancel)
        {
            UnturnedZombieDamageEvent @event;

            if (damageAmount >= (ushort) ZombieHealth.GetValue(zombie))
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

        private void Events_OnZombieDead(Zombie zombie, Vector3 ragdoll, ERagdollEffect ragdollEffect)
        {
            UnturnedZombieDeadEvent @event = new UnturnedZombieDeadEvent(zombie, ragdoll, ragdollEffect);

            Emit(@event);
        }

        private void Events_OnZombieSpawn(Zombie zombie)
        {
            UnturnedZombieSpawnEvent @event = new UnturnedZombieSpawnEvent(zombie);

            Emit(@event);
        }

        private delegate void ZombieDamage(Zombie zombie, ref ushort damageAmount, ref Vector3 ragdoll,
            ref ERagdollEffect ragdollEffect,
            ref bool trackKill, ref bool dropLoot, ref EZombieStunOverride stunOverride, out bool cancel);
        private static event ZombieDamage OnZombieDamage;

        private delegate void ZombieSpawn(Zombie zombie);
        private static event ZombieSpawn OnZombieSpawn;

        private delegate void ZombieDead(Zombie zombie, Vector3 ragdoll, ERagdollEffect ragdollEffect);
        private static event ZombieDead OnZombieDead;

        [HarmonyPatch]
        private class ZombiePatches
        {
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

            [HarmonyPatch(typeof(Zombie), "tellAlive")]
            [HarmonyPostfix]
            private static void TellAlive(Zombie __instance)
            {
                OnZombieSpawn?.Invoke(__instance);
            }

            [HarmonyPatch(typeof(ZombieManager), "addZombie")]
            [HarmonyPostfix]
            private static void AddZombie(byte bound)
            {
                Zombie zombie = ZombieManager.regions[bound].zombies.LastOrDefault();

                if (zombie != null)
                {
                    OnZombieSpawn?.Invoke(zombie);
                }
            }

            [HarmonyPatch(typeof(Zombie), "tellDead")]
            [HarmonyPostfix]
            private static void TellDead(Zombie __instance, Vector3 newRagdoll, ERagdollEffect ragdollEffect)
            {
                OnZombieDead?.Invoke(__instance, newRagdoll, ragdollEffect);
            }
        }
    }
}
