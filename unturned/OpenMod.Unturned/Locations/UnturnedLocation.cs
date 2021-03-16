using OpenMod.UnityEngine.Extensions;
using SDG.Unturned;
using System.Numerics;

namespace OpenMod.Unturned.Locations
{
    public class UnturnedLocation
    {
        public UnturnedLocation(LocationNode node)
        {
            Node = node;
        }

        public string Name => Node.name;

        public Vector3 Position => Node.point.ToSystemVector();

        public LocationNode Node { get; }
    }
}
