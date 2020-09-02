using HarmonyLib;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Users;
using OpenMod.Unturned.Events;
using OpenMod.Unturned.Players;
using SDG.Unturned;
using UnityEngine;

namespace OpenMod.Unturned.Animals.Events
{
    internal class AnimalsEventsListener : UnturnedEventsListener
    {
        public AnimalsEventsListener(IOpenModHost openModHost,
            IEventBus eventBus,
            IUserManager userManager) : base(openModHost, eventBus, userManager)
        {

        }

        public override void Subscribe()
        {
            OnAnimalAdd += Events_OnAnimalAdd;
            OnAnimalRevive += Events_OnAnimalRevive;
            OnAnimalDamage += Events_OnAnimalDamage;
            OnAnimalDead += Events_OnAnimalDead;
            OnAnimalFlee += Events_OnAnimalFlee;
            OnAnimalAttackPoint += Events_OnAnimalAttackPoint;
            OnAnimalAttackPlayer += Events_OnAnimalAttackPlayer;
        }

        public override void Unsubscribe()
        {
            OnAnimalAdd -= Events_OnAnimalAdd;
            OnAnimalRevive -= Events_OnAnimalRevive;
            OnAnimalDamage -= Events_OnAnimalDamage;
            OnAnimalDead -= Events_OnAnimalDead;
            OnAnimalFlee -= Events_OnAnimalFlee;
            OnAnimalAttackPoint -= Events_OnAnimalAttackPoint;
            OnAnimalAttackPlayer -= Events_OnAnimalAttackPlayer;
        }

        private void Events_OnAnimalAdd(Animal nativeAnimal)
        {
            UnturnedAnimal animal = new UnturnedAnimal(nativeAnimal);

            UnturnedAnimalAddEvent @event = new UnturnedAnimalAddEvent(animal);

            Emit(@event);
        }

        private void Events_OnAnimalRevive(Animal nativeAnimal)
        {
            UnturnedAnimal animal = new UnturnedAnimal(nativeAnimal);

            UnturnedAnimalReviveEvent @event = new UnturnedAnimalReviveEvent(animal);

            Emit(@event);
        }

        private void Events_OnAnimalDamage(Animal nativeAnimal, ref ushort amount, ref Vector3 ragdoll,
            ref ERagdollEffect ragdollEffect, ref bool trackKill, ref bool dropLoot, out bool cancel)
        {
            UnturnedAnimal animal = new UnturnedAnimal(nativeAnimal);

            UnturnedAnimalDamageEvent @event;

            if (amount >= animal.Health) 
            {
                @event = new UnturnedAnimalDyingEvent(animal, amount, ragdoll, ragdollEffect, trackKill, dropLoot);
            }
            else
            {
                @event = new UnturnedAnimalDamageEvent(animal, amount, ragdoll, ragdollEffect, trackKill, dropLoot);
            }

            Emit(@event);

            amount = @event.DamageAmount;
            ragdoll = @event.Ragdoll;
            ragdollEffect = @event.RagdollEffect;
            trackKill = @event.TrackKill;
            dropLoot = @event.DropLoot;
            cancel = @event.IsCancelled;
        }

        private void Events_OnAnimalDead(Animal nativeAnimal, Vector3 ragdoll, ERagdollEffect ragdollEffect)
        {
            UnturnedAnimal animal = new UnturnedAnimal(nativeAnimal);

            UnturnedAnimalDeadEvent @event = new UnturnedAnimalDeadEvent(animal, ragdoll, ragdollEffect);

            Emit(@event);
        }

        private void Events_OnAnimalFlee(Animal nativeAnimal, ref Vector3 direction, ref bool sendToPack, out bool cancel)
        {
            UnturnedAnimal animal = new UnturnedAnimal(nativeAnimal);

            UnturnedAnimalFleeEvent @event = new UnturnedAnimalFleeEvent(animal, direction, sendToPack);

            Emit(@event);

            direction = @event.Direction;
            sendToPack = @event.SendToPack;
            cancel = @event.IsCancelled;
        }

        private void Events_OnAnimalAttackPoint(Animal nativeAnimal, ref Vector3 point, ref bool sendToPack, out bool cancel)
        {
            UnturnedAnimal animal = new UnturnedAnimal(nativeAnimal);

            UnturnedAnimalAttackPointEvent @event = new UnturnedAnimalAttackPointEvent(animal, point, sendToPack);

            Emit(@event);

            point = @event.Point;
            sendToPack = @event.SendToPack;
            cancel = @event.IsCancelled;
        }

        private void Events_OnAnimalAttackPlayer(Animal nativeAnimal, ref Player nativePlayer, ref bool sendToPack,
            out bool cancel)
        {
            UnturnedAnimal animal = new UnturnedAnimal(nativeAnimal);

            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedAnimalAttackPlayerEvent @event = new UnturnedAnimalAttackPlayerEvent(animal, player, sendToPack);

            Emit(@event);

            nativePlayer = @event.Player?.Player;
            sendToPack = @event.SendToPack;
            cancel = @event.IsCancelled;
        }

        private delegate void AnimalSpawn(Animal nativeAnimal);
        private static event AnimalSpawn OnAnimalAdd;
        private static event AnimalSpawn OnAnimalRevive;

        private delegate void AnimalDamage(Animal nativeAnimal, ref ushort amount, ref Vector3 ragdoll,
            ref ERagdollEffect ragdollEffect, ref bool trackKill, ref bool dropLoot, out bool cancel);
        private static event AnimalDamage OnAnimalDamage;

        private delegate void AnimalDead(Animal nativeAnimal, Vector3 ragdoll, ERagdollEffect ragdollEffect);
        private static event AnimalDead OnAnimalDead;

        private delegate void AnimalFlee(Animal nativeAnimal, ref Vector3 direction, ref bool sendToPack, out bool cancel);
        private static event AnimalFlee OnAnimalFlee;

        private delegate void AnimalAttackPoint(Animal nativeAnimal, ref Vector3 point, ref bool sendToPack, out bool cancel);
        private static event AnimalAttackPoint OnAnimalAttackPoint;

        private delegate void AnimalAttackPlayer(Animal nativeAnimal, ref Player player, ref bool sendToPack,
            out bool cancel);
        private static event AnimalAttackPlayer OnAnimalAttackPlayer;

        [HarmonyPatch]
        private class Patches
        {
            [HarmonyPatch(typeof(AnimalManager), "addAnimal")]
            [HarmonyPostfix]
            private static void AddAnimal(Animal __result)
            {
                if (__result != null)
                {
                    OnAnimalAdd?.Invoke(__result);
                }
            }

            [HarmonyPatch(typeof(Animal), "tellAlive")]
            [HarmonyPostfix]
            private static void TellAlive(Animal __instance)
            {
                OnAnimalRevive?.Invoke(__instance);
            }

            [HarmonyPatch(typeof(Animal), "askDamage")]
            [HarmonyPrefix]
            private static bool AskDamage(Animal __instance, ref ushort amount, ref Vector3 newRagdoll,
                ref ERagdollEffect ragdollEffect, ref bool trackKill, ref bool dropLoot)
            {
                bool cancel = false;

                if (amount == 0 || __instance.isDead) return false;

                OnAnimalDamage?.Invoke(__instance, ref amount, ref newRagdoll, ref ragdollEffect, ref trackKill,
                    ref dropLoot, out cancel);

                return !cancel;
            }

            [HarmonyPatch(typeof(Animal), "tellDead")]
            [HarmonyPostfix]
            private static void TellDead(Animal __instance, Vector3 newRagdoll, ERagdollEffect ragdollEffect)
            {
                OnAnimalDead?.Invoke(__instance, newRagdoll, ragdollEffect);
            }

            [HarmonyPatch(typeof(Animal), "alertDirection")]
            [HarmonyPrefix]
            private static bool AlertDirection(Animal __instance, ref Vector3 newDirection, ref bool sendToPack)
            {
                // Fleeing from given direction
                bool cancel = false;

                OnAnimalFlee?.Invoke(__instance, ref newDirection, ref sendToPack, out cancel);

                return !cancel;
            }

            [HarmonyPatch(typeof(Animal), "alertGoToPoint")]
            [HarmonyPrefix]
            private static bool AlertGoToPoint(Animal __instance, ref Vector3 point, ref bool sendToPack)
            {
                // Attacking point
                bool cancel = false;

                OnAnimalAttackPoint?.Invoke(__instance, ref point, ref sendToPack, out cancel);

                return !cancel;
            }

            [HarmonyPatch(typeof(Animal), "alertPlayer")]
            [HarmonyPrefix]
            private static bool AlertPlayer(Animal __instance, ref Player player, ref bool sendToPack)
            {
                // Attacking player
                bool cancel = false;

                OnAnimalAttackPlayer?.Invoke(__instance, ref player, ref sendToPack, out cancel);

                return !cancel;
            }
        }
    }
}
