using OpenMod.Unturned.Players;
using Steamworks;
using UnityEngine;

namespace OpenMod.Unturned.Building.Events
{
    public class UnturnedStructureTransformingEvent : UnturnedBuildableTransformingEvent, IUnturnedStructureEvent
    {
        public new UnturnedStructureBuildable Buildable => (UnturnedStructureBuildable) base.Buildable;

        public UnturnedStructureTransformingEvent(UnturnedStructureBuildable buildable, UnturnedPlayer instigator,
            CSteamID instigatorId, Vector3 point, Quaternion rotation) : base(buildable, instigator, instigatorId,
            point, rotation)
        {
        }
    }
}
