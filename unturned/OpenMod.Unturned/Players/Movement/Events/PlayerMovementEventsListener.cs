using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using OpenMod.UnityEngine.Extensions;
using OpenMod.Unturned.Events;
using OpenMod.Unturned.Patching;
using SDG.Unturned;
using UnityEngine;

namespace OpenMod.Unturned.Players.Movement.Events
{
    [UsedImplicitly]
    internal class PlayerMovementEventsListener : UnturnedPlayerEventsListener
    {
        public PlayerMovementEventsListener(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            SubscribePlayer(
                static player => ref player.stance.onStanceUpdated,
                player => () => OnStanceUpdated(player)
            );
            SubscribePlayer(
                static player => ref player.movement.onSafetyUpdated,
                player => safe => OnSafetyUpdated(player, safe)
            );
        }

        public override void Subscribe()
        {
            PlayerAnimator.OnGestureChanged_Global += PlayerAnimator_OnGestureChanged_Global;
            OnTeleporting += Events_OnTeleporting;
            OnPlayerJumped += PlayerMovementEventsListener_OnPlayerJumped;
        }

        public override void Unsubscribe()
        {
            PlayerAnimator.OnGestureChanged_Global -= PlayerAnimator_OnGestureChanged_Global;
            OnTeleporting -= Events_OnTeleporting;
            OnPlayerJumped -= PlayerMovementEventsListener_OnPlayerJumped;
        }

        private void PlayerAnimator_OnGestureChanged_Global(PlayerAnimator animator, EPlayerGesture gesture)
        {
            var player = GetUnturnedPlayer(animator.player)!;

            var @event = new UnturnedPlayerGestureUpdatedEvent(player, gesture);

            Emit(@event);
        }

        private void Events_OnTeleporting(Player nativePlayer, ref Vector3 position, ref float yaw, ref bool cancel) // lgtm [cs/too-many-ref-parameters]
        {
            var player = GetUnturnedPlayer(nativePlayer)!;

            var @event = new UnturnedPlayerTeleportingEvent(player, position.ToSystemVector(), yaw)
            {
                IsCancelled = cancel
            };

            Emit(@event);

            position = @event.Position.ToUnityVector();
            yaw = @event.Yaw;
            cancel = @event.IsCancelled;
        }

        private void OnStanceUpdated(Player nativePlayer)
        {
            var player = GetUnturnedPlayer(nativePlayer)!;

            var @event = new UnturnedPlayerStanceUpdatedEvent(player);

            Emit(@event);
        }

        private void OnSafetyUpdated(Player nativePlayer, bool safe)
        {
            var player = GetUnturnedPlayer(nativePlayer)!;

            var @event = new UnturnedPlayerSafetyUpdatedEvent(player, safe);

            Emit(@event);
        }

        private void PlayerMovementEventsListener_OnPlayerJumped(Player nativePlayer)
        {
            var player = GetUnturnedPlayer(nativePlayer)!;

            var @event = new UnturnedPlayerJumpedEvent(player);

            Emit(@event);
        }

        private delegate void Teleporting(Player player, ref Vector3 position, ref float yaw, ref bool cancel);
        private static event Teleporting? OnTeleporting;

        private delegate void PlayerJumped(Player player);
        private static event PlayerJumped? OnPlayerJumped;

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
            [HarmonyPatch(typeof(Player), nameof(Player.teleportToLocationUnsafe))]
            [HarmonyPrefix]
            // ReSharper disable once InconsistentNaming
            public static bool TeleportToLocationUnsafe(Player __instance, ref Vector3 position, ref float yaw)
            {
                var cancel = false;

                OnTeleporting?.Invoke(__instance, ref position, ref yaw, ref cancel);

                return !cancel;
            }

            [HarmonyPatch(typeof(PlayerMovement), nameof(PlayerMovement.simulate),
                argumentTypes: new[] { typeof(uint), typeof(int), typeof(int), typeof(int),
                    typeof(float), typeof(float), typeof(bool), typeof(bool), typeof(float) })]
            [HarmonyTranspiler]
            public static IEnumerable<CodeInstruction> JumpPatch(IEnumerable<CodeInstruction> instructions)
            {
                var askTireMethod = typeof(PlayerLife).GetMethod(nameof(PlayerLife.askTire), BindingFlags.Public | BindingFlags.Instance)
                    ?? throw new NullReferenceException("AskTire method is null");

                foreach (var instruction in instructions)
                {
                    if (instruction.Calls(askTireMethod))
                    {
                        yield return instruction;

                        // Loads the PlayerMovement onto the evaluation stack.
                        yield return new(OpCodes.Ldarg_0);
                        yield return CodeInstruction.Call(() => OnJumpedEvent(null!));

                        continue;
                    }

                    yield return instruction;
                }
            }

            public static void OnJumpedEvent(PlayerMovement movement)
            {
                OnPlayerJumped?.Invoke(movement.player);
            }
        }
    }
}