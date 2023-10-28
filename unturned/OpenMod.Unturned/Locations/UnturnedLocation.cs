using OpenMod.UnityEngine.Extensions;
using SDG.Unturned;
using System;
using System.Numerics;

namespace OpenMod.Unturned.Locations
{
    public class UnturnedLocation
    {
        [Obsolete("Use constructor with " + nameof(LocationDevkitNode) + " parameter", true)]
        public UnturnedLocation(LocationNode node)
        {
            throw new Exception($"Use constructor with {nameof(LocationDevkitNode)} parameter");
        }

        public UnturnedLocation(LocationDevkitNode node)
        {
            DevkitNode = node;
        }

        public string Name => DevkitNode.locationName;

        public Vector3 Position => DevkitNode.transform.position.ToSystemVector();

        [Obsolete("Use " + nameof(DevkitNode) + " parameter", true)]
        public LocationNode Node => new(DevkitNode.transform.position, DevkitNode.locationName);

        public LocationDevkitNode DevkitNode { get; }
    }
}
