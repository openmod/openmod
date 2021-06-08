extern alias JetBrainsAnnotations;
using JetBrainsAnnotations::JetBrains.Annotations;
using OpenMod.Unturned.Events;
using SDG.Unturned;
using Steamworks;
using System;

namespace OpenMod.Unturned.Players.Quests.Events
{
    [UsedImplicitly]
    internal abstract class PlayerQuestsEventsListener : UnturnedPlayerEventsListener
    {
        public PlayerQuestsEventsListener(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override void Subscribe()
        {
            PlayerQuests.onGroupChanged += OnGroupChanged;
        }

        private void OnGroupChanged(PlayerQuests sender, CSteamID oldGroupID, EPlayerGroupRank oldGroupRank, CSteamID newGroupID, EPlayerGroupRank newGroupRank)
        {
            var player = GetUnturnedPlayer(sender.player)!;

            var @event = new UnturnedPlayerGroupChangedEvent(player, oldGroupID, oldGroupRank, newGroupID, newGroupRank);

            Emit(@event);
        }

        public override void Unsubscribe()
        {
            PlayerQuests.onGroupChanged -= OnGroupChanged;
        }
    }
}
