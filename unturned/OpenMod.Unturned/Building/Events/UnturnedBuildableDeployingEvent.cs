using OpenMod.API.Eventing;

using UnityEngine;

using Event = OpenMod.Core.Eventing.Event;

namespace OpenMod.Unturned.Building.Events
{
    public class UnturnedBuildableDeployingEvent : Event, ICancellableEvent
    {

        public UnturnedBuildableAsset Asset { get; }

        public Vector3 Point { get; set; }

        public Quaternion Rotation { get; set; }

        public ulong Owner { get; set; }

        public ulong Group { get; set; }

        public bool IsCancelled { get; set; }

        public UnturnedBuildableDeployingEvent(UnturnedBuildableAsset asset, Vector3 point, Quaternion rotation, ulong owner, ulong @group)
        {
            Asset = asset;
            Point = point;
            Rotation = rotation;
            Owner = owner;
            Group = @group;
        }
    }
}
