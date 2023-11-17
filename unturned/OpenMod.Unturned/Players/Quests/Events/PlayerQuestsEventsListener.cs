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
        }

        public override void Subscribe()
        {
            PlayerQuests.onGroupChanged += OnGroupOrRankChanged;
            PlayerQuests.onAnyFlagChanged += OnAnyFlagChanged;

            OnPlayerJoiningGroup += Events_OnPlayerJoiningGroup;
            OnPlayerLeavingGroup += Events_OnPlayerLeavingGroup;
        }

        public override void Unsubscribe()
        {
            PlayerQuests.onGroupChanged -= OnGroupOrRankChanged;
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

        private void OnGroupOrRankChanged(PlayerQuests sender, CSteamID oldGroupId, EPlayerGroupRank oldGroupRank, CSteamID newGroupId, EPlayerGroupRank newGroupRank)
        {
            var player = GetUnturnedPlayer(sender.player)!;

            if (oldGroupRank != newGroupRank)
            {
                var groupRankChangedEvent = new UnturnedPlayerGroupRankChangedEvent(player, oldGroupRank, newGroupRank);

                Emit(groupRankChangedEvent);
            }

            if (newGroupId != oldGroupId)
            {
                var groupIdChangedEvent = new UnturnedPlayerGroupIdChangedEvent(player, oldGroupId, newGroupId);

                Emit(groupIdChangedEvent);
            }

            var groupIdOrRankChangedEvent = new UnturnedPlayerGroupOrRankChangedEvent(player, oldGroupId, oldGroupRank, newGroupId, newGroupRank);

            Emit(groupIdOrRankChangedEvent);
        }

        private void Events_OnPlayerJoiningGroup(PlayerQuests playerQuests, CSteamID newGroupId, EPlayerGroupRank newGroupRank, ref bool isCancelled)
        {
            var player = GetUnturnedPlayer(playerQuests.player)!;

            var @event =
                new UnturnedPlayerJoiningGroupEvent(player, newGroupId, newGroupRank)
                {
                    IsCancelled = isCancelled
                };

            Emit(@event);

            isCancelled = @event.IsCancelled;
        }

        private void Events_OnPlayerLeavingGroup(PlayerQuests playerQuests, ref bool isCancelled)
        {
            var player = GetUnturnedPlayer(playerQuests.player)!;

            var @event = new UnturnedPlayerLeavingGroupEvent(player)
                {
                    IsCancelled = isCancelled
            };

            Emit(@event);

            isCancelled = @event.IsCancelled;
        }

        private delegate void PlayerJoiningGroup(PlayerQuests playerQuests, CSteamID newGroupId, EPlayerGroupRank newGroupRank, ref bool isCancelled);

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
            [HarmonyPatch(typeof(PlayerQuests), nameof(PlayerQuests.leaveGroup))]
            [HarmonyPrefix]
            // ReSharper disable once InconsistentNaming
            public static bool LeaveGroup(PlayerQuests __instance, bool force)
            {
                if (!force && (!__instance.canChangeGroupMembership || !__instance.hasPermissionToLeaveGroup))
                {
                    return true;
                }

                var isCancelled = false;

                OnPlayerLeavingGroup?.Invoke(__instance, ref isCancelled);

                return !isCancelled;
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(PlayerQuests), nameof(PlayerQuests.ServerAssignToGroup))]
            [HarmonyPrefix]
            // ReSharper disable InconsistentNaming
            public static bool ServerAssignToGroup(PlayerQuests __instance, CSteamID newGroupID, EPlayerGroupRank newRank, bool bypassMemberLimit)
                // ReSharper restore InconsistentNaming
            {
                var groupInfo = GroupManager.getGroupInfo(newGroupID);

                if (groupInfo == null || !(bypassMemberLimit || groupInfo.hasSpaceForMoreMembersInGroup))
                {
                    return true;
                }

                var isCancelled = false;

                OnPlayerJoiningGroup?.Invoke(__instance, newGroupID, newRank, ref isCancelled);

                return !isCancelled;
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(PlayerQuests), nameof(PlayerQuests.ServerAssignToMainGroup))]
            [HarmonyPrefix]
            // ReSharper disable once InconsistentNaming
            public static bool ServerAssignToMainGroup(PlayerQuests __instance)
            {
                var isCancelled = false;
                var newGroupId = __instance.channel.owner.playerID.group;

                OnPlayerJoiningGroup?.Invoke(__instance, newGroupId, EPlayerGroupRank.MEMBER, ref isCancelled);

                return !isCancelled;
            }
        }
    }
}