using System;
using System.Threading.Tasks;
using SDG.Unturned;
using UnityEngine;

namespace OpenMod.Unturned.World
{
    public interface IDeath
    {
        public Vector3 Location {get;}
        public TimeSpan Time { get; }
        Task<LocationNode> NearestLocation();

        public EDeathCause Cause { get; }

    }
}