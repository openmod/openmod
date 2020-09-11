using HarmonyLib;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Users;
using OpenMod.Extensions.Games.Abstractions.Entities;
using OpenMod.UnityEngine.Extensions;
using OpenMod.Unturned.Events;
using SDG.Unturned;
using Steamworks;
using System.Linq;
using UnityEngine;

namespace OpenMod.Unturned.Players.Life.Events
{
    internal class PlayerLifeEventsListener : UnturnedPlayerEventsListener
    {
        public PlayerLifeEventsListener(IOpenModHost openModHost,
            IEventBus eventBus,
            IUserManager userManager) : base(openModHost, eventBus, userManager)
        {

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

        public override void SubscribePlayer(Player player)
        {
            player.life.onHurt += OnHurt;
        }

        public override void UnsubscribePlayer(Player player)
        {
            player.life.onHurt -= OnHurt;
        }

        private IDamageSource GetDamageSource(UnturnedPlayer victim, byte amount, EDeathCause cause, ELimb limb,
            CSteamID killer)
        {
            IDamageSource source = null;

            // TODO: Split DoDamage into all possible forms of damage to allow multiple IDamageSource inputs
            if (killer != CSteamID.Nil)
            {
                CSteamID temp = killer;

                source = GetUnturnedPlayer(Provider.clients.FirstOrDefault(
                    x => x.playerID.steamID == temp));
            }

            return source;
        }

        private void Events_OnDoDamage(Player nativePlayer, ref byte amount,
            ref EDeathCause cause, ref ELimb limb,
            ref CSteamID killer, ref bool trackKill,
            ref Vector3 ragdoll, ref ERagdollEffect ragdollEffect,
            ref bool canCauseBleeding, out bool cancel)
        {
            cancel = false;

            if (amount == 0 || nativePlayer.life.isDead) return;

            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            IDamageSource source = GetDamageSource(player, amount, cause, limb, killer);

            UnturnedPlayerDamagingEvent @event;

            if (amount >= nativePlayer.life.health)
            {
                @event = new UnturnedPlayerDyingEvent(player, amount, cause, limb, killer,
                    source, trackKill, ragdoll.ToSystemVector(), ragdollEffect, canCauseBleeding);
            }
            else
            {
                @event = new UnturnedPlayerDamagingEvent(player, amount, cause, limb, killer,
                    source, trackKill, ragdoll.ToSystemVector(), ragdollEffect, canCauseBleeding);
            }

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
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            IDamageSource source = GetDamageSource(player, damage, cause, limb, killer);

            UnturnedPlayerDamagedEvent @event =
                new UnturnedPlayerDamagedEvent(player, damage, cause, limb, killer, source);

            Emit(@event);
        }

        private void OnPlayerDeath(PlayerLife sender, EDeathCause cause, ELimb limb, CSteamID instigator)
        {
            UnturnedPlayer player = GetUnturnedPlayer(sender.player);

            UnturnedPlayerDeadEvent @event = new UnturnedPlayerDeadEvent(player, cause, limb, instigator);

            Emit(@event);
        }

        private void OnPlayerLifeUpdated(Player nativePlayer)
        {
            if (!nativePlayer.life.isDead)
            {
                UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

                UnturnedPlayerRevivedEvent @event = new UnturnedPlayerRevivedEvent(player);

                Emit(@event);
            }
        }

        private delegate void DoDamageHandler(Player player, ref byte amount,
            ref EDeathCause cause, ref ELimb limb,
            ref CSteamID killer, ref bool trackKill,
            ref Vector3 ragdoll, ref ERagdollEffect ragdollEffect,
            ref bool canCauseBleeding, out bool cancel);

        private static event DoDamageHandler OnDoDamage;


        [HarmonyPatch]
        private class Patches
        {
            [HarmonyPatch(typeof(PlayerLife), "doDamage")]
            [HarmonyPrefix]
            private static bool DoDamage(PlayerLife __instance, ref byte amount,
                ref EDeathCause newCause, ref ELimb newLimb,
                ref CSteamID newKiller, ref bool trackKill,
                ref Vector3 newRagdoll, ref ERagdollEffect newRagdollEffect,
                ref bool canCauseBleeding)
            {
                bool cancel = false;

                OnDoDamage?.Invoke(__instance.player, ref amount,
                    ref newCause, ref newLimb,
                    ref newKiller, ref trackKill,
                    ref newRagdoll, ref newRagdollEffect,
                    ref canCauseBleeding, out cancel);

                return !cancel;
            }
        }
    }
}
