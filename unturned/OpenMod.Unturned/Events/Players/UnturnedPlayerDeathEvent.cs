using OpenMod.Unturned.Entities;
using SDG.Unturned;
using Steamworks;

namespace OpenMod.Unturned.Events.Players
{
    public class UnturnedPlayerDeathEvent : UnturnedPlayerEvent
    {
        public UnturnedPlayerDeathEvent(UnturnedPlayer player, EDeathCause cause, ELimb limb, CSteamID instigator) : base(player)
        {
            DeathCause = cause;
            Limb = limb;
            Instigator = instigator;
        }

        public EDeathCause DeathCause { get; }

        public ELimb Limb { get; }

        public CSteamID Instigator { get; }
    }
}
