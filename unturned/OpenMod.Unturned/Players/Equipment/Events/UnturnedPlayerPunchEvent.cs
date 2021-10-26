using OpenMod.Unturned.Events;

using SDG.Unturned;

namespace OpenMod.Unturned.Players.Equipment.Events
{
    public class UnturnedPlayerPunchEvent : UnturnedPlayerEvent
    {

        public EPlayerPunch Punch { get; }

        public UnturnedPlayerPunchEvent(UnturnedPlayer player, EPlayerPunch punch) : base(player)
            => Punch = punch;

    }
}