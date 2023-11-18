using HarmonyLib;
using JetBrains.Annotations;
using OpenMod.Extensions.Games.Abstractions.Entities;
using OpenMod.UnityEngine.Extensions;
using OpenMod.Unturned.Events;
using OpenMod.Unturned.Patching;
using SDG.Unturned;
using Steamworks;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace OpenMod.Unturned.Players.Life.Events
{
    [UsedImplicitly]
    internal class PlayerLifeEventsListener : UnturnedPlayerEventsListener
    {
        public PlayerLifeEventsListener(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            SubscribePlayer<Hurt>(
                static (player, handler) => player.life.onHurt += handler,
                static (player, handler) => player.life.onHurt -= handler,
                _ => OnHurt
            );

            SubscribePlayer<PlayerLife.FallDamageRequestHandler>(
                static (player, handler) => player.life.OnFallDamageRequested += handler,
                static (player, handler) => player.life.OnFallDamageRequested -= handler,
                _ => OnFallDamageRequested
            );
        }

        public override void Subscribe()
        {
            OnDoDamage += Events_OnDoDamage;

            PlayerLife.onPlayerDied += OnPlayerDeath;
            PlayerLife.onPlayerLifeUpdated += OnPlayerLifeUpdated;
        }

        public override void Unsubscribe()
        {
            OnDoDamage -= Events_OnDoDamage;

            PlayerLife.onPlayerDied -= OnPlayerDeath;
            PlayerLife.onPlayerLifeUpdated -= OnPlayerLifeUpdated;
        }

        private IDamageSource? GetDamageSource(CSteamID killer)
        {
            IDamageSource? source = null;

            // TODO: Split DoDamage into all possible forms of damage to allow multiple IDamageSource inputs
            if (killer != CSteamID.Nil)
            {
                var temp = killer;

                source = GetUnturnedPlayer(Provider.clients.FirstOrDefault(x => x.playerID.steamID == temp));
            }

            return source;
        }

        private void Events_OnDoDamage(Player nativePlayer, ref byte amount, // lgtm [cs/too-many-ref-parameters]
            ref EDeathCause cause, ref ELimb limb,
            ref CSteamID killer, ref bool trackKill,
            ref Vector3 ragdoll, ref ERagdollEffect ragdollEffect,
            ref bool canCauseBleeding, ref bool cancel)
        {
            if (amount == 0 || nativePlayer.life.isDead)
            {
                return;
            }

            var player = GetUnturnedPlayer(nativePlayer)!;
            var source = GetDamageSource(killer);

            var @event = amount >= nativePlayer.life.health
                ? new UnturnedPlayerDyingEvent(player, amount, cause, limb, killer,
                    source, trackKill, ragdoll.ToSystemVector(), ragdollEffect, canCauseBleeding)
                : new UnturnedPlayerDamagingEvent(player, amount, cause, limb, killer,
                    source, trackKill, ragdoll.ToSystemVector(), ragdollEffect, canCauseBleeding);

            @event.IsCancelled = cancel;

            Emit(@event);

            amount = @event.DamageAmount;
            cause = @event.Cause;
            limb = @event.Limb;
            killer = @event.Killer;
            trackKill = @event.TrackKill;
            ragdoll = @event.Ragdoll.ToUnityVector();
            ragdollEffect = @event.RagdollEffect;
            canCauseBleeding = @event.CanCauseBleeding;
            cancel = @event.IsCancelled;
        }

        private void OnHurt(Player nativePlayer, byte damage, Vector3 force, EDeathCause cause, ELimb limb, CSteamID killer)
        {
            var player = GetUnturnedPlayer(nativePlayer)!;
            var source = GetDamageSource(killer);

            var @event = new UnturnedPlayerDamagedEvent(player, damage, cause, limb, killer, source);

            Emit(@event);
        }

        private void OnFallDamageRequested(PlayerLife playerLife, float velocity, ref float damage, ref bool shouldBreakLegs)
        {
            var player = GetUnturnedPlayer(playerLife.player)!;
            var source = GetDamageSource(player.SteamId);

            var @event = new UnturnedPlayerFallDamagingEvent(player, velocity, (byte)damage, source, shouldBreakLegs);

            Emit(@event);

            if (@event.IsCancelled)
            {
                damage = 0;
                shouldBreakLegs = false;
            }
            else
            {
                damage = @event.DamageAmount;
                shouldBreakLegs = @event.ShouldBreakLegs;
            }
        }

        private void OnPlayerDeath(PlayerLife sender, EDeathCause cause, ELimb limb, CSteamID instigator)
        {
            var player = GetUnturnedPlayer(sender.player)!;
            var @event = new UnturnedPlayerDeathEvent(player, cause, limb, instigator);

            Emit(@event);
        }

        private void OnPlayerLifeUpdated(Player nativePlayer)
        {
            if (nativePlayer.life.isDead)
            {
                return;
            }

            var player = GetUnturnedPlayer(nativePlayer)!;
            var @event = new UnturnedPlayerRevivedEvent(player);

            Emit(@event);
        }

        private delegate void DoDamageHandler(Player player, ref byte amount,
            ref EDeathCause cause, ref ELimb limb,
            ref CSteamID killer, ref bool trackKill,
            ref Vector3 ragdoll, ref ERagdollEffect ragdollEffect,
            ref bool canCauseBleeding, ref bool cancel);

        private static event DoDamageHandler? OnDoDamage;


        [UsedImplicitly]
        [HarmonyPatch]
        internal static class Patches
        {
            [HarmonyCleanup]
            public static Exception? Cleanup(Exception ex, MethodBase original)
            {
                HarmonyExceptionHandler.ReportCleanupException(typeof(Patches), ex, original);
                return null;
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(PlayerLife), "doDamage")]
            [HarmonyPrefix]
            // ReSharper disable once InconsistentNaming
            public static bool DoDamage(PlayerLife __instance, ref byte amount, // lgtm [cs/too-many-ref-parameters]
                ref EDeathCause newCause, ref ELimb newLimb,
                ref CSteamID newKiller, ref bool trackKill,
                ref Vector3 newRagdoll, ref ERagdollEffect newRagdollEffect,
                ref bool canCauseBleeding)
            {
                var cancel = false;

                OnDoDamage?.Invoke(__instance.player, ref amount,
                    ref newCause, ref newLimb,
                    ref newKiller, ref trackKill,
                    ref newRagdoll, ref newRagdollEffect,
                    ref canCauseBleeding, ref cancel);

                return !cancel;
            }
        }
    }
}
