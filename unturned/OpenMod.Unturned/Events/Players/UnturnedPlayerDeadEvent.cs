using OpenMod.Unturned.Entities;
using UnityEngine;

namespace OpenMod.Unturned.Events.Players
{
    public class UnturnedPlayerDeadEvent : UnturnedPlayerEvent
    {
        public UnturnedPlayerDeadEvent(UnturnedPlayer player, Vector3 ragdoll, byte ragdollEffect) : base(player)
        {
            Ragdoll = ragdoll;
            RagdollEffect = ragdollEffect;
        }

        public Vector3 Ragdoll { get; }

        public byte RagdollEffect { get; }
    }
}
