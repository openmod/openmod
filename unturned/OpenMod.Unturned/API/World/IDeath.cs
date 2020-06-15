using System;
using System.Threading.Tasks;
using SDG.Unturned;
using UnityEngine;

namespace OpenMod.Unturned.World
{
    public interface IDeath
    {
        public IDeathLocation Location {get;}
        public TimeSpan Time { get; }
        Task<LocationNode> NearestLocation();

        public EDeathCause Cause { get; }

    }
}