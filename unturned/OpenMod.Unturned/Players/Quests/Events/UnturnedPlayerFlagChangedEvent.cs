extern alias JetBrainsAnnotations;
using OpenMod.Unturned.Events;
using SDG.Unturned;

namespace OpenMod.Unturned.Players.Quests.Events
{
    public class UnturnedPlayerFlagChangedEvent : UnturnedPlayerEvent
    {
        public PlayerQuests Quests { get; }
        public PlayerQuestFlag Flag { get;  }

        public UnturnedPlayerFlagChangedEvent(UnturnedPlayer player, PlayerQuests quests, PlayerQuestFlag flag) :
            base(player)
        {
            Quests = quests;
            Flag = flag;
        }
    }
}
