using OpenMod.API.Eventing;
using OpenMod.Unturned.Events;
using UnityEngine;

namespace OpenMod.Unturned.Players.Events.Movement
{
    public class UnturnedPlayerTeleportEvent : UnturnedPlayerEvent, ICancellableEvent
    {
        public Vector3 Position { get; set; }

        public float Yaw { get; set; }

        public bool IsCancelled { get; set; }

        public UnturnedPlayerTeleportEvent(UnturnedPlayer player, Vector3 position, float yaw) : base(player)
        {
            Position = position;
            Yaw = yaw;
        }
    }
}
