using OpenMod.Unturned.Events;

namespace OpenMod.Unturned.Players.Animator.Events
{
    public class UnturnedPlayerLeanUpdatedEvent : UnturnedPlayerEvent
    {
        public LeanType Lean { get; }
        public UnturnedPlayerLeanUpdatedEvent(UnturnedPlayer player, LeanType lean) : base(player)
        {
            Lean = lean;
        }
    }
}