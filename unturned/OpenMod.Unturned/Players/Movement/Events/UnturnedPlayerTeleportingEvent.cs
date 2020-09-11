using OpenMod.API.Eventing;
using OpenMod.Unturned.Events;
using System.Numerics;

namespace OpenMod.Unturned.Players.Movement.Events
{
    public class UnturnedPlayerTeleportingEvent : UnturnedPlayerEvent, ICancellableEvent
    {
        public Vector3 Position { get; set; }

        public float Yaw { get; set; }

        public bool IsCancelled { get; set; }

        public UnturnedPlayerTeleportingEvent(UnturnedPlayer player, Vector3 position, float yaw) : base(player)
        {
            Position = position;
            Yaw = yaw;
        }
    }
}
