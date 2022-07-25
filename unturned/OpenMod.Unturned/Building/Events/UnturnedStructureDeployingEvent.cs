using SDG.Unturned;

using UnityEngine;

namespace OpenMod.Unturned.Building.Events
{
    /// <summary>
    /// Event that is emitted when a structure is being placed.
    /// Special note: the game completely disregards any rotation in X and Z axis
    /// </summary>
    public class UnturnedStructureDeployingEvent : UnturnedBuildableDeployingEvent
    {
        public ItemStructureAsset StructureAsset { get; }

        public UnturnedStructureDeployingEvent(ItemStructureAsset asset, Vector3 point, Quaternion rotation, ulong owner, ulong group)
            : base(new UnturnedBuildableAsset(asset), point, rotation, owner, group)
        {
            StructureAsset = asset;
        }
    }
}
