using OpenMod.Extensions.Games.Abstractions.Entities;
using OpenMod.Extensions.Games.Abstractions.Players;
using OpenMod.Unturned.Events;
using SDG.Unturned;
using Steamworks;

namespace OpenMod.Unturned.Players.Life.Events
{
    public class UnturnedPlayerDamagedEvent : UnturnedPlayerEvent, IPlayerDamagedEvent
    {
        public byte DamageAmount { get; }

        double IPlayerDamagedEvent.DamageAmount => DamageAmount;

        public EDeathCause Cause { get; }

        public ELimb Limb { get; set; }

        public CSteamID Killer { get; set; }

        public IDamageSource? DamageSource { get; }

        public UnturnedPlayerDamagedEvent(UnturnedPlayer player, byte damageAmount, EDeathCause cause, ELimb limb, CSteamID killer, IDamageSource? damageSource) : base(player)
        {
            DamageAmount = damageAmount;
            Cause = cause;
            Limb = limb;
            Killer = killer;
            DamageSource = damageSource;
        }
    }
}
