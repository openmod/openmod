using JetBrains.Annotations;
using OpenMod.Unturned.Events;
using SDG.Unturned;
using Steamworks;
using System;

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
        }

        public override void Unsubscribe()
        {
            PlayerQuests.onGroupChanged -= OnGroupChanged;
            PlayerQuests.onAnyFlagChanged -= OnAnyFlagChanged;
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
    }
}