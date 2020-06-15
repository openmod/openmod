using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenMod.Unturned.Commands;
using OpenMod.Unturned.World;
using SDG.Unturned;
using UnityEngine;

namespace OpenMod.Unturned.API.Player
{
    public class PlayerDeathLocation : IDeathLocation
    {
        /// <inheritdoc />
        public Vector3 Location { get; set; }

        public UnturnedPlayerCommandActor Player;

        /// <inheritdoc />
        async Task<LocationNode> ILocation.GetNearestLocationAsync()
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

                if (!(await Player.GetDistanceFromAsync(result.point) > await Player.GetDistanceFromAsync(node.point))) continue;
                
                result = node;
                
            }
            
            return result;
        }
        
    }
}