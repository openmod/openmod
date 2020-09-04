using HarmonyLib;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Users;
using OpenMod.UnityEngine.Extensions;
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
            OnAnimalAdded += Events_OnAnimalAdded;
            OnAnimalRevived += Events_OnAnimalRevived;
            OnAnimalDamaging += Events_OnAnimalDamaging;
            OnAnimalDead += Events_OnAnimalDead;
            OnAnimalFleeing += Events_OnAnimalFleeing;
            OnAnimalAttackingPoint += Events_OnAnimalAttackingPoint;
            OnAnimalAttackingPlayer += Events_OnAnimalAttackingPlayer;
        }

        public override void Unsubscribe()
        {
            OnAnimalAdded -= Events_OnAnimalAdded;
            OnAnimalRevived -= Events_OnAnimalRevived;
            OnAnimalDamaging -= Events_OnAnimalDamaging;
            OnAnimalDead -= Events_OnAnimalDead;
            OnAnimalFleeing -= Events_OnAnimalFleeing;
            OnAnimalAttackingPoint -= Events_OnAnimalAttackingPoint;
            OnAnimalAttackingPlayer -= Events_OnAnimalAttackingPlayer;
        }

        private void Events_OnAnimalAdded(Animal nativeAnimal)
        {
            UnturnedAnimal animal = new UnturnedAnimal(nativeAnimal);

            UnturnedAnimalAddedEvent @event = new UnturnedAnimalAddedEvent(animal);

            Emit(@event);
        }

        private void Events_OnAnimalRevived(Animal nativeAnimal)
        {
            UnturnedAnimal animal = new UnturnedAnimal(nativeAnimal);

            UnturnedAnimalRevivedEvent @event = new UnturnedAnimalRevivedEvent(animal);

            Emit(@event);
        }

        private void Events_OnAnimalDamaging(Animal nativeAnimal, ref ushort amount, ref Vector3 ragdoll,
            ref ERagdollEffect ragdollEffect, ref bool trackKill, ref bool dropLoot, out bool cancel)
        {
            UnturnedAnimal animal = new UnturnedAnimal(nativeAnimal);

            UnturnedAnimalDamagingEvent @event;

            if (amount >= animal.Health)
            {
                @event = new UnturnedAnimalDyingEvent(animal, amount, ragdoll.ToSystemVector(), ragdollEffect, trackKill, dropLoot);
            }
            else
            {
                @event = new UnturnedAnimalDamagingEvent(animal, amount, ragdoll.ToSystemVector(), ragdollEffect, trackKill, dropLoot);
            }

            Emit(@event);

            amount = @event.DamageAmount;
            ragdoll = @event.Ragdoll.ToUnityVector();
            ragdollEffect = @event.RagdollEffect;
            trackKill = @event.TrackKill;
            dropLoot = @event.DropLoot;
            cancel = @event.IsCancelled;
        }

        private void Events_OnAnimalDead(Animal nativeAnimal, Vector3 ragdoll, ERagdollEffect ragdollEffect)
        {
            UnturnedAnimal animal = new UnturnedAnimal(nativeAnimal);

            UnturnedAnimalDeadEvent @event = new UnturnedAnimalDeadEvent(animal, ragdoll.ToSystemVector(), ragdollEffect);

            Emit(@event);
        }

        private void Events_OnAnimalFleeing(Animal nativeAnimal, ref Vector3 direction, ref bool sendToPack, out bool cancel)
        {
            UnturnedAnimal animal = new UnturnedAnimal(nativeAnimal);

            UnturnedAnimalFleeingEvent @event = new UnturnedAnimalFleeingEvent(animal, direction.ToSystemVector(), sendToPack);

            Emit(@event);

            direction = @event.Direction.ToUnityVector();
            sendToPack = @event.SendToPack;
            cancel = @event.IsCancelled;
        }

        private void Events_OnAnimalAttackingPoint(Animal nativeAnimal, ref Vector3 point, ref bool sendToPack, out bool cancel)
        {
            UnturnedAnimal animal = new UnturnedAnimal(nativeAnimal);

            UnturnedAnimalAttackingPointEvent @event = new UnturnedAnimalAttackingPointEvent(animal, point.ToSystemVector(), sendToPack);

            Emit(@event);

            point = @event.Point.ToUnityVector();
            sendToPack = @event.SendToPack;
            cancel = @event.IsCancelled;
        }

        private void Events_OnAnimalAttackingPlayer(Animal nativeAnimal, ref Player nativePlayer, ref bool sendToPack,
            out bool cancel)
        {
            UnturnedAnimal animal = new UnturnedAnimal(nativeAnimal);

            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedAnimalAttackingPlayerEvent @event = new UnturnedAnimalAttackingPlayerEvent(animal, player, sendToPack);

            Emit(@event);

            nativePlayer = @event.Player?.Player;
            sendToPack = @event.SendToPack;
            cancel = @event.IsCancelled;
        }

        private delegate void AnimalSpawned(Animal nativeAnimal);
        private static event AnimalSpawned OnAnimalAdded;
        private static event AnimalSpawned OnAnimalRevived;

        private delegate void AnimalDamaging(Animal nativeAnimal, ref ushort amount, ref Vector3 ragdoll,
            ref ERagdollEffect ragdollEffect, ref bool trackKill, ref bool dropLoot, out bool cancel);
        private static event AnimalDamaging OnAnimalDamaging;

        private delegate void AnimalDead(Animal nativeAnimal, Vector3 ragdoll, ERagdollEffect ragdollEffect);
        private static event AnimalDead OnAnimalDead;

        private delegate void AnimalFleeing(Animal nativeAnimal, ref Vector3 direction, ref bool sendToPack, out bool cancel);
        private static event AnimalFleeing OnAnimalFleeing;

        private delegate void AnimalAttackingPoint(Animal nativeAnimal, ref Vector3 point, ref bool sendToPack, out bool cancel);
        private static event AnimalAttackingPoint OnAnimalAttackingPoint;

        private delegate void AnimalAttackingPlayer(Animal nativeAnimal, ref Player player, ref bool sendToPack,
            out bool cancel);
        private static event AnimalAttackingPlayer OnAnimalAttackingPlayer;

        [HarmonyPatch]
        private class Patches
        {
            [HarmonyPatch(typeof(AnimalManager), "addAnimal")]
            [HarmonyPostfix]
            private static void AddAnimal(Animal __result)
            {
                if (__result != null)
                {
                    OnAnimalAdded?.Invoke(__result);
                }
            }

            [HarmonyPatch(typeof(Animal), "tellAlive")]
            [HarmonyPostfix]
            private static void TellAlive(Animal __instance)
            {
                OnAnimalRevived?.Invoke(__instance);
            }

            [HarmonyPatch(typeof(Animal), "askDamage")]
            [HarmonyPrefix]
            private static bool AskDamage(Animal __instance, ref ushort amount, ref Vector3 newRagdoll,
                ref ERagdollEffect ragdollEffect, ref bool trackKill, ref bool dropLoot)
            {
                bool cancel = false;

                if (amount == 0 || __instance.isDead) return false;

                OnAnimalDamaging?.Invoke(__instance, ref amount, ref newRagdoll, ref ragdollEffect, ref trackKill,
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

                OnAnimalFleeing?.Invoke(__instance, ref newDirection, ref sendToPack, out cancel);

                return !cancel;
            }

            [HarmonyPatch(typeof(Animal), "alertGoToPoint")]
            [HarmonyPrefix]
            private static bool AlertGoToPoint(Animal __instance, ref Vector3 point, ref bool sendToPack)
            {
                // Attacking point
                bool cancel = false;

                OnAnimalAttackingPoint?.Invoke(__instance, ref point, ref sendToPack, out cancel);

                return !cancel;
            }

            [HarmonyPatch(typeof(Animal), "alertPlayer")]
            [HarmonyPrefix]
            private static bool AlertPlayer(Animal __instance, ref Player newPlayer, ref bool sendToPack)
            {
                // Attacking player
                bool cancel = false;

                OnAnimalAttackingPlayer?.Invoke(__instance, ref newPlayer, ref sendToPack, out cancel);

                return !cancel;
            }
        }
    }
}
