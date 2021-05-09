extern alias JetBrainsAnnotations;
using HarmonyLib;
using JetBrainsAnnotations::JetBrains.Annotations;
using OpenMod.UnityEngine.Extensions;
using OpenMod.Unturned.Events;
using SDG.Unturned;
using System;
using System.Reflection;
using SDG.Framework.Devkit;
using Steamworks;
using UnityEngine;

// ReSharper disable InconsistentNaming

namespace OpenMod.Unturned.Animals.Events
{
    [UsedImplicitly]
    internal class AnimalsEventsListener : UnturnedEventsListener
    {
        private static readonly FieldInfo s_BumperVehicle;

        static AnimalsEventsListener()
        {
            s_BumperVehicle = typeof(Bumper).GetField("vehicle", BindingFlags.Instance | BindingFlags.NonPublic)!;
        }

        public AnimalsEventsListener(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override void Subscribe()
        {
            OnAnimalAdded += Events_OnAnimalAdded;
            OnAnimalRevived += Events_OnAnimalRevived;
            OnAnimalDead += Events_OnAnimalDead;
            OnAnimalFleeing += Events_OnAnimalFleeing;
            OnAnimalAttackingPoint += Events_OnAnimalAttackingPoint;
            OnAnimalAttackingPlayer += Events_OnAnimalAttackingPlayer;
            DamageTool.damageAnimalRequested += DamageToolOndamageAnimalRequested;
        }

        public override void Unsubscribe()
        {
            OnAnimalAdded -= Events_OnAnimalAdded;
            OnAnimalRevived -= Events_OnAnimalRevived;
            OnAnimalDead -= Events_OnAnimalDead;
            OnAnimalFleeing -= Events_OnAnimalFleeing;
            OnAnimalAttackingPoint -= Events_OnAnimalAttackingPoint;
            OnAnimalAttackingPlayer -= Events_OnAnimalAttackingPlayer;
            DamageTool.damageAnimalRequested -= DamageToolOndamageAnimalRequested;
        }

        private void Events_OnAnimalAdded(Animal nativeAnimal)
        {
            var animal = new UnturnedAnimal(nativeAnimal);

            var @event = new UnturnedAnimalAddedEvent(animal);

            Emit(@event);
        }

        private void Events_OnAnimalRevived(Animal nativeAnimal)
        {
            var animal = new UnturnedAnimal(nativeAnimal);

            var @event = new UnturnedAnimalRevivedEvent(animal);

            Emit(@event);
        }

        private void DamageToolOndamageAnimalRequested(ref DamageAnimalParameters parameters, ref bool shouldallow)
        {
            var animal = new UnturnedAnimal(parameters.animal);
            var amount = (ushort)Math.Min(ushort.MaxValue, Math.Floor(parameters.damage * parameters.times));
            var ragdoll = (amount * parameters.direction).ToSystemVector();
            var ragdollEffect = parameters.ragdollEffect;

            var instigator = CSteamID.Nil;
            switch (parameters.instigator)
            {
                case KillVolume or Barrier or InteractableTrap or InteractableSentry:
                    instigator = Provider.server;
                    break;
                case Bumper bumper:
                {
                    var vehicle = (InteractableVehicle)s_BumperVehicle.GetValue(bumper);
                    instigator = vehicle is not null and { isDriven: true }
                        ? vehicle.passengers[0].player.playerID.steamID
                        : Provider.server;

                    break;
                }
                case Player player:
                    instigator = player.channel.owner.playerID.steamID;
                    break;
            }

            var @event = amount >= animal.Health
                ? new UnturnedAnimalDyingEvent(animal, amount, ragdoll, ragdollEffect, instigator)
                : new UnturnedAnimalDamagingEvent(animal, amount, ragdoll, ragdollEffect, instigator);

            @event.IsCancelled = !shouldallow;

            Emit(@event);

            shouldallow = !@event.IsCancelled;
            parameters.damage = @event.DamageAmount;
            parameters.times = 1;
            parameters.direction = @event.Ragdoll.ToUnityVector();
            parameters.ragdollEffect = @event.RagdollEffect;
        }

        private void Events_OnAnimalDead(Animal nativeAnimal, Vector3 ragdoll, ERagdollEffect ragdollEffect)
        {
            var animal = new UnturnedAnimal(nativeAnimal);

            var @event = new UnturnedAnimalDeadEvent(animal, ragdoll.ToSystemVector(), ragdollEffect);

            Emit(@event);
        }

        private void Events_OnAnimalFleeing(Animal nativeAnimal, ref Vector3 direction, ref bool sendToPack,
            ref bool cancel) // lgtm [cs/too-many-ref-parameters]
        {
            var animal = new UnturnedAnimal(nativeAnimal);

            var @event = new UnturnedAnimalFleeingEvent(animal, direction.ToSystemVector(), sendToPack)
            {
                IsCancelled = cancel
            };

            Emit(@event);

            direction = @event.Direction.ToUnityVector();
            sendToPack = @event.SendToPack;
            cancel = @event.IsCancelled;
        }

        private void Events_OnAnimalAttackingPoint(Animal nativeAnimal, ref Vector3 point, ref bool sendToPack,
            ref bool cancel) // lgtm [cs/too-many-ref-parameters]
        {
            var animal = new UnturnedAnimal(nativeAnimal);

            var @event = new UnturnedAnimalAttackingPointEvent(animal, point.ToSystemVector(), sendToPack)
            {
                IsCancelled = cancel
            };

            Emit(@event);

            point = @event.Point.ToUnityVector();
            sendToPack = @event.SendToPack;
            cancel = @event.IsCancelled;
        }

        private void Events_OnAnimalAttackingPlayer(Animal nativeAnimal, ref Player nativePlayer,
            ref bool sendToPack, // lgtm [cs/too-many-ref-parameters]
            ref bool cancel)
        {
            var animal = new UnturnedAnimal(nativeAnimal);

            var player = GetUnturnedPlayer(nativePlayer);

            var @event = new UnturnedAnimalAttackingPlayerEvent(animal, player!, sendToPack)
            {
                IsCancelled = cancel
            };

            Emit(@event);

            nativePlayer = @event.Player.Player;
            sendToPack = @event.SendToPack;
            cancel = @event.IsCancelled;
        }

        private delegate void AnimalSpawned(Animal nativeAnimal);

        private static event AnimalSpawned? OnAnimalAdded;
        private static event AnimalSpawned? OnAnimalRevived;

        private delegate void AnimalDead(Animal nativeAnimal, Vector3 ragdoll, ERagdollEffect ragdollEffect);

        private static event AnimalDead? OnAnimalDead;

        private delegate void AnimalFleeing(Animal nativeAnimal, ref Vector3 direction, ref bool sendToPack,
            ref bool cancel);

        private static event AnimalFleeing? OnAnimalFleeing;

        private delegate void AnimalAttackingPoint(Animal nativeAnimal, ref Vector3 point, ref bool sendToPack,
            ref bool cancel);

        private static event AnimalAttackingPoint? OnAnimalAttackingPoint;

        private delegate void AnimalAttackingPlayer(Animal nativeAnimal, ref Player player, ref bool sendToPack,
            ref bool cancel);

        private static event AnimalAttackingPlayer? OnAnimalAttackingPlayer;

        [UsedImplicitly]
        [HarmonyPatch]
        internal static class Patches
        {
            [UsedImplicitly]
            [HarmonyPatch(typeof(AnimalManager), "addAnimal")]
            [HarmonyPostfix]
            public static void AddAnimal(Animal __result)
            {
                if (__result != null)
                {
                    OnAnimalAdded?.Invoke(__result);
                }
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(Animal), "tellAlive")]
            [HarmonyPostfix]
            public static void TellAlive(Animal __instance)
            {
                OnAnimalRevived?.Invoke(__instance);
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(Animal), "tellDead")]
            [HarmonyPostfix]
            public static void TellDead(Animal __instance, Vector3 newRagdoll, ERagdollEffect ragdollEffect)
            {
                OnAnimalDead?.Invoke(__instance, newRagdoll, ragdollEffect);
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(Animal), "alertDirection")]
            [HarmonyPrefix]
            public static bool AlertDirection(Animal __instance, ref Vector3 newDirection, ref bool sendToPack)
            {
                // Fleeing from given direction
                var cancel = false;

                OnAnimalFleeing?.Invoke(__instance, ref newDirection, ref sendToPack, ref cancel);

                return !cancel;
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(Animal), "alertGoToPoint")]
            [HarmonyPrefix]
            public static bool AlertGoToPoint(Animal __instance, ref Vector3 point, ref bool sendToPack)
            {
                // Attacking point
                var cancel = false;

                OnAnimalAttackingPoint?.Invoke(__instance, ref point, ref sendToPack, ref cancel);

                return !cancel;
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(Animal), "alertPlayer")]
            [HarmonyPrefix]
            public static bool AlertPlayer(Animal __instance, ref Player newPlayer, ref bool sendToPack)
            {
                // Attacking player
                var cancel = false;

                OnAnimalAttackingPlayer?.Invoke(__instance, ref newPlayer, ref sendToPack, ref cancel);

                return !cancel;
            }
        }
    }
}