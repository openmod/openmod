using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NuGet.Versioning;
using OpenMod.Unturned.World;
using SDG.Unturned;
using UnityEngine;

namespace OpenMod.Unturned.API.Player
{
    public class PlayerDeath : IDeath
    {
        public IDeathLocation Location
        {
            get => Location;
            set
            {
                PlayerDeathLocation = (PlayerDeathLocation) value;
                Location = value;
            }
        }

        public IUnturnedPlayerActor Player;
        public TimeSpan Time { get; set; }
        public PlayerDeathLocation PlayerDeathLocation { get; set; } 

        
        public async Task<LocationNode> NearestLocation()
        {
            LocationNode result = null;

            List<LocationNode> nodesInLevel = (List<LocationNode>) LevelNodes.nodes.Where(n => n.type == ENodeType.LOCATION);

            foreach (LocationNode node in nodesInLevel)
            {
                if (result == null)
                {
                    result = node;
                    continue;
                }

                if (!(GetDistanceFrom(result.point) > GetDistanceFrom(node.point))) continue;
                
                result = node;
                
            }
            
            return result;
        }
        
        public EDeathCause Cause { get; set; }

        private float GetDistanceFrom(Vector3 point) => Vector3.Distance(Player.Player.transform.position, point);



    }
}