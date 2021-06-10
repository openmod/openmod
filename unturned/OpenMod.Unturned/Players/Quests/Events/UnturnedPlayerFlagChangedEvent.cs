using OpenMod.Unturned.Events;
using SDG.Unturned;

namespace OpenMod.Unturned.Players.Quests.Events
{
    public class UnturnedPlayerFlagChangedEvent : UnturnedPlayerEvent
    {
        public PlayerQuestFlag Flag { get;  }

        public UnturnedPlayerFlagChangedEvent(UnturnedPlayer player, PlayerQuestFlag flag) :
            base(player)
        {
            Flag = flag;
        }
    }
}
