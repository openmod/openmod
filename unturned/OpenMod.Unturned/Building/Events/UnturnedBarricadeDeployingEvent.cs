using SDG.Unturned;

using UnityEngine;

namespace OpenMod.Unturned.Building.Events
{
    public class UnturnedBarricadeDeployingEvent : UnturnedBuildableDeployingEvent
    {
        public Transform Hit { get; }

        public ItemBarricadeAsset BarricadeAsset { get; }

        public UnturnedBarricadeDeployingEvent(ItemBarricadeAsset asset, Transform hit, Vector3 point, Quaternion rotation,
            ulong owner, ulong group) : base(new UnturnedBuildableAsset(asset), point, rotation, owner, group)
        {
            BarricadeAsset = asset;
            Hit = hit;
        }
    }
}
