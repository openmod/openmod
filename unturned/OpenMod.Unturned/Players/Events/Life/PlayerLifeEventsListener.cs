using HarmonyLib;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Users;
using OpenMod.Extensions.Games.Abstractions.Entities;
using OpenMod.Unturned.Events;
using SDG.Unturned;
using Steamworks;
using System.Linq;
using UnityEngine;

namespace OpenMod.Unturned.Players.Events.Life
{
    internal class PlayerLifeEventsListener : UnturnedEventsListener
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

        private void Events_OnDoDamage(Player nativePlayer, ref byte amount,
            ref EDeathCause cause, ref ELimb limb,
            ref CSteamID killer, ref bool trackKill,
            ref Vector3 ragdoll, ref ERagdollEffect ragdollEffect,
            ref bool canCauseBleeding, out bool cancel)
        {
            cancel = false;

            if (amount == 0 || nativePlayer.life.isDead) return; 

            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            IDamageSource source = null;

            // TODO: Split DoDamage into all possible forms of damage to allow multiple IDamageSource inputs
            if (killer != CSteamID.Nil)
            {
                CSteamID temp = killer;

                source = GetUnturnedPlayer(Provider.clients.FirstOrDefault(
                    x => x.playerID.steamID == temp));
            }

            UnturnedPlayerDamageEvent @event;

            if (amount >= nativePlayer.life.health)
            {
                @event = new UnturnedPlayerDyingEvent(player, amount, cause, limb, killer,
                    source, trackKill, ragdoll, ragdollEffect, canCauseBleeding);
            }
            else
            {
                @event = new UnturnedPlayerDamageEvent(player, amount, cause, limb, killer,
                    source, trackKill, ragdoll, ragdollEffect, canCauseBleeding);
            }

            Emit(@event);

            amount = @event.DamageAmount;
            cause = @event.Cause;
            limb = @event.Limb;
            killer = @event.Killer;
            trackKill = @event.TrackKill;
            ragdoll = @event.Ragdoll;
            ragdollEffect = @event.RagdollEffect;
            canCauseBleeding = @event.CanCauseBleeding;
            cancel = @event.IsCancelled;
        }

        private void OnPlayerDeath(PlayerLife sender, EDeathCause cause, ELimb limb, CSteamID instigator)
        {
            UnturnedPlayer player = GetUnturnedPlayer(sender.player);

            UnturnedPlayerDeathEvent @event = new UnturnedPlayerDeathEvent(player, cause, limb, instigator);

            Emit(@event);
        }

        private void OnPlayerLifeUpdated(Player nativePlayer)
        {
            if (!nativePlayer.life.isDead)
            {
                UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

                UnturnedPlayerReviveEvent @event = new UnturnedPlayerReviveEvent(player);

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
