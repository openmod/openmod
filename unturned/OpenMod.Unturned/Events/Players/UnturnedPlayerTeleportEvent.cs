using OpenMod.API.Eventing;
using OpenMod.Unturned.Entities;
using UnityEngine;

namespace OpenMod.Unturned.Events.Players
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
