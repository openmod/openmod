using HarmonyLib;
using JetBrains.Annotations;
using OpenMod.Unturned.Events;
using OpenMod.Unturned.Patching;
using SDG.Unturned;
using Steamworks;
using System;
using System.Reflection;

namespace OpenMod.Unturned.Players.Quests.Events
{
    [UsedImplicitly]
    internal class PlayerQuestsEventsListener : UnturnedPlayerEventsListener
    {
        public PlayerQuestsEventsListener(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            SubscribePlayer<GroupRankChangedHandler>(
                static (player, handler) => player.quests.groupRankChanged += handler,
                static (player, handler) => player.quests.groupRankChanged -= handler,
                player => OnGroupRankChanged
            );
        }

        public override void Subscribe()
        {
            PlayerQuests.onGroupChanged += OnGroupChanged;
            PlayerQuests.onAnyFlagChanged += OnAnyFlagChanged;
            OnPlayerJoiningGroup += Events_OnPlayerJoiningGroup;
            OnPlayerLeavingGroup += Events_OnPlayerLeavingGroup;
        }

        public override void Unsubscribe()
        {
            PlayerQuests.onGroupChanged -= OnGroupChanged;
            PlayerQuests.onAnyFlagChanged -= OnAnyFlagChanged;
            OnPlayerJoiningGroup -= Events_OnPlayerJoiningGroup;
            OnPlayerLeavingGroup -= Events_OnPlayerLeavingGroup;
        }

        private void OnAnyFlagChanged(PlayerQuests quests, PlayerQuestFlag flag)
        {
            var player = GetUnturnedPlayer(quests.player)!;

            var @event =
                new UnturnedPlayerFlagChangedEvent(player, flag);

            Emit(@event);
        }

        private void OnGroupChanged(PlayerQuests sender, CSteamID oldGroupId, EPlayerGroupRank oldGroupRank,
            CSteamID newGroupId, EPlayerGroupRank newGroupRank)
        {
            var player = GetUnturnedPlayer(sender.player)!;

            var @event =
                new UnturnedPlayerGroupChangedEvent(player, oldGroupId, oldGroupRank, newGroupId, newGroupRank);

            Emit(@event);
        }

        private void OnGroupRankChanged(PlayerQuests sender, EPlayerGroupRank oldGroupRank,
            EPlayerGroupRank newGroupRank)
        {
            var player = GetUnturnedPlayer(sender.player)!;

            var @event =
                new UnturnedPlayerGroupRankChangedEvent(player, oldGroupRank, newGroupRank);

            Emit(@event);
        }

        private void Events_OnPlayerJoiningGroup(PlayerQuests playerQuests, CSteamID newGroupID, EPlayerGroupRank newGroupRank, ref bool isCancelled)
        {
            var player = GetUnturnedPlayer(playerQuests.player)!;

            var @event =
                new UnturnedPlayerJoiningGroupEvent(player, newGroupID, newGroupRank);

            Emit(@event);

            isCancelled = @event.IsCancelled;
        }

        private void Events_OnPlayerLeavingGroup(PlayerQuests playerQuests, ref bool isCancelled)
        {
            var player = GetUnturnedPlayer(playerQuests.player)!;

            var @event =
                new UnturnedPlayerLeavingGroupEvent(player);

            Emit(@event);

            isCancelled = @event.IsCancelled;
        }

        private delegate void PlayerJoiningGroup(PlayerQuests playerQuests, CSteamID newGroupID, EPlayerGroupRank newGroupRank, ref bool isCancelled);

        private delegate void PlayerLeavingGroup(PlayerQuests playerQuests, ref bool isCancelled);

        private static event PlayerJoiningGroup? OnPlayerJoiningGroup;

        private static event PlayerLeavingGroup? OnPlayerLeavingGroup;

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
            [HarmonyPatch(typeof(PlayerQuests), nameof(PlayerQuests.ReceiveGroupState))]
            [HarmonyPrefix]
            // ReSharper disable once InconsistentNaming
            public static bool ReceiveGroupState(PlayerQuests __instance, CSteamID newGroupID, EPlayerGroupRank newGroupRank)
            {
                var isCancelled = false;

                OnPlayerJoiningGroup?.Invoke(__instance, newGroupID, newGroupRank, ref isCancelled);

                return !isCancelled;
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(PlayerQuests), nameof(PlayerQuests.ReceiveLeaveGroupRequest))]
            [HarmonyPrefix]
            // ReSharper disable once InconsistentNaming
            public static bool ReceiveLeaveGroupRequest(PlayerQuests __instance)
            {
                var isCancelled = false;

                OnPlayerLeavingGroup?.Invoke(__instance, ref isCancelled);

                return !isCancelled;
            }
        }
    }
}