using OpenMod.Unturned.Events;

namespace OpenMod.Unturned.Players.Events.Skills
{
    public class UnturnedPlayerExperienceUpdateEvent : UnturnedPlayerEvent
    {
        public uint Experience { get; }

        public UnturnedPlayerExperienceUpdateEvent(UnturnedPlayer player, uint experience) : base(player)
        {
            Experience = experience;
        }
    }
}
