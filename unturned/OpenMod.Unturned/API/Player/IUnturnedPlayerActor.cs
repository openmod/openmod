using System.Numerics;
using System.Threading.Tasks;
using OpenMod.API.Commands;
using OpenMod.Unturned.World;
using SDG.Unturned;
using Steamworks;
using Vector3 = UnityEngine.Vector3;

namespace OpenMod.Unturned.API.Player
{
    public interface IUnturnedPlayerActor : ICommandActor
    {
        public SDG.Unturned.Player Player { get; }
        public SteamPlayer SteamPlayer { get; }
        public CSteamID SteamId { get; }
        public PlayerDeath LastDeath { get;  }
        
        Task<float> GetDistanceFromAsync(Vector3 point); 

        
        
    }
}
