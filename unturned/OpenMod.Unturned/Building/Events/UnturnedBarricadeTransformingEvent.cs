using OpenMod.Unturned.Players;
using Steamworks;
using UnityEngine;

namespace OpenMod.Unturned.Building.Events
{
    public class UnturnedBarricadeTransformingEvent : UnturnedBuildableTransformingEvent, IUnturnedBarricadeEvent
    {
        public new UnturnedBarricadeBuildable Buildable => (UnturnedBarricadeBuildable) base.Buildable;

        public UnturnedBarricadeTransformingEvent(UnturnedBarricadeBuildable buildable, UnturnedPlayer instigator,
            CSteamID instigatorId, Vector3 point, Quaternion rotation) : base(buildable, instigator, instigatorId,
            point, rotation)
        {
        }
    }
}
