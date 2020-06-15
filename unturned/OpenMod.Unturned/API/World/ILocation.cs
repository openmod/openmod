using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SDG.Unturned;
using UnityEngine;

namespace OpenMod.Unturned.World
{
    public interface ILocation
    {
        public Vector3 Location { get; set; }

        Task<LocationNode> GetNearestLocationAsync();
    }
}