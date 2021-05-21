using OpenMod.Extensions.Games.Abstractions.Building;
using OpenMod.Unturned.Players;
using Steamworks;
using UnityEngine;

namespace OpenMod.Unturned.Building.Events
{
    public class UnturnedBuildableTransformingEvent : UnturnedBuildableEvent, IBuildableTransformingEvent
    {
        public UnturnedPlayer Instigator { get; }

        public CSteamID InstigatorId { get; }

        public Vector3 Point { get; set; }

        public Quaternion Rotation { get; set; }

        public bool IsCancelled { get; set; }

        public UnturnedBuildableTransformingEvent(UnturnedBuildable buildable, UnturnedPlayer instigator,
            CSteamID instigatorId, Vector3 point, Quaternion rotation) : base(buildable)
        {
            Instigator = instigator;
            InstigatorId = instigatorId;
            Point = point;
            Rotation = rotation;
        }
    }
}
