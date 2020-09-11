using OpenMod.Unturned.Events;
using SDG.Unturned;

namespace OpenMod.Unturned.Players.Movement.Events
{
    public class UnturnedPlayerGestureUpdatedEvent : UnturnedPlayerEvent
    {
        public EPlayerGesture Gesture { get; }

        public UnturnedPlayerGestureUpdatedEvent(UnturnedPlayer player, EPlayerGesture gesture) : base(player)
        {
            Gesture = gesture;
        }
    }
}
