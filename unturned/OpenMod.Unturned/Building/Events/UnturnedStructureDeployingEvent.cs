using SDG.Unturned;

using UnityEngine;

namespace OpenMod.Unturned.Building.Events
{
    public class UnturnedStructureDeployingEvent : UnturnedBuildableDeployingEvent
    {
        public ItemStructureAsset StructureAsset { get; }

        public UnturnedStructureDeployingEvent(ItemStructureAsset asset, Vector3 point, Quaternion rotation, ulong owner, ulong @group)
            : base(new UnturnedBuildableAsset(asset), point, rotation, owner, @group)
        {
            StructureAsset = asset;
        }
    }
}
