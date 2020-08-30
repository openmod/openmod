using OpenMod.Unturned.Entities;

namespace OpenMod.Unturned.Events.Players.Skills
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
