using HarmonyLib;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Users;
using OpenMod.Unturned.Entities;
using SDG.Unturned;
using Steamworks;
using UnityEngine;

namespace OpenMod.Unturned.Events.Players
{
    internal class PlayerEventsListener : UnturnedPlayerEventsListener
    {
        public PlayerEventsListener(IOpenModHost openModHost,
            IEventBus eventBus,
            IUserManager userManager) : base(openModHost, eventBus, userManager)
        {

        }

        public override void Subscribe()
        {
            Provider.onEnemyConnected += OnPlayerConnected;
            Provider.onEnemyDisconnected += OnPlayerDisconnected;

            OnDoDamage += Events_OnDoDamage;
            OnTeleport += Events_OnTeleport;

            UseableConsumeable.onPerformingAid += OnPerformingAid;
            PlayerLife.onPlayerDied += OnPlayerDeath;
        }

        public override void Unsubscribe()
        {
            Provider.onEnemyConnected -= OnPlayerConnected;
            Provider.onEnemyDisconnected -= OnPlayerDisconnected;

            OnDoDamage -= Events_OnDoDamage;
            OnTeleport -= Events_OnTeleport;

            UseableConsumeable.onPerformingAid -= OnPerformingAid;
            PlayerLife.onPlayerDied -= OnPlayerDeath;
        }

        public override void SubscribePlayer(Player player)
        {
            player.stance.onStanceUpdated += () => OnStanceUpdated(player);
        }

        public override void UnsubscribePlayer(Player player)
        {
            player.stance.onStanceUpdated -= () => OnStanceUpdated(player);
        }

        private void OnPlayerConnected(SteamPlayer steamPlayer)
        {
            UnturnedPlayer player = GetUnturnedPlayer(steamPlayer);

            UnturnedPlayerConnectEvent @event = new UnturnedPlayerConnectEvent(player);

            Emit(@event);
        }

        private void OnPlayerDisconnected(SteamPlayer steamPlayer)
        {
            UnturnedPlayer player = GetUnturnedPlayer(steamPlayer);

            UnturnedPlayerDisconnectEvent @event = new UnturnedPlayerDisconnectEvent(player);

            Emit(@event);
        }

        private void Events_OnDoDamage(Player nativePlayer, ref byte amount,
            ref EDeathCause cause, ref ELimb limb,
            ref CSteamID killer, ref bool trackKill,
            ref Vector3 ragdoll, ref ERagdollEffect ragdollEffect,
            ref bool canCauseBleeding, out bool cancel)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerDamageEvent @event = new UnturnedPlayerDamageEvent(player, amount, cause, limb, killer,
                trackKill, ragdoll, ragdollEffect, canCauseBleeding);

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

        private void Events_OnTeleport(Player nativePlayer, ref Vector3 position, ref float yaw, out bool cancel)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerTeleportEvent @event = new UnturnedPlayerTeleportEvent(player, position, yaw);

            Emit(@event);

            position = @event.Position;
            yaw = @event.Yaw;
            cancel = @event.IsCancelled;
        }

        private void Events_OnDead(Player nativePlayer, Vector3 ragdoll, byte ragdollEffect)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerDeadEvent @event = new UnturnedPlayerDeadEvent(player, ragdoll, ragdollEffect);

            Emit(@event);
        }

        private void OnPerformingAid(Player nativeInstigator, Player nativeTarget, ItemConsumeableAsset asset, ref bool shouldAllow)
        {
            UnturnedPlayer instigator = GetUnturnedPlayer(nativeInstigator);
            UnturnedPlayer target = GetUnturnedPlayer(nativeTarget);

            UnturnedPlayerPerformAidEvent @event = new UnturnedPlayerPerformAidEvent(instigator, target, asset);

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
        }

        private void OnStanceUpdated(Player nativePlayer)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerStanceUpdateEvent @event = new UnturnedPlayerStanceUpdateEvent(player);

            Emit(@event);
        }

        private void OnPlayerDeath(PlayerLife sender, EDeathCause cause, ELimb limb, CSteamID instigator)
        {
            UnturnedPlayer player = GetUnturnedPlayer(sender.player);

            UnturnedPlayerDeathEvent @event = new UnturnedPlayerDeathEvent(player, cause, limb, instigator);

            Emit(@event);
        }

        private delegate void DoDamageHandler(Player player, ref byte amount,
            ref EDeathCause cause, ref ELimb limb,
            ref CSteamID killer, ref bool trackKill,
            ref Vector3 ragdoll, ref ERagdollEffect ragdollEffect,
            ref bool canCauseBleeding, out bool cancel);
        private static event DoDamageHandler OnDoDamage;

        private delegate void TeleportHandler(Player player,
            ref Vector3 position, ref float yaw, out bool cancel);
        private static event TeleportHandler OnTeleport;

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

            [HarmonyPatch(typeof(Player), "teleportToLocationUnsafe")]
            [HarmonyPrefix]
            private static bool TeleportToLocationUnsafe(Player __instance, ref Vector3 position, ref float yaw)
            {
                bool cancel = false;

                OnTeleport?.Invoke(__instance, ref position, ref yaw, out cancel);

                return !cancel;
            }
        }
    }
}
