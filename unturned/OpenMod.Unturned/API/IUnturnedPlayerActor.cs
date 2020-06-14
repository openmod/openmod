using OpenMod.API.Commands;
using SDG.Unturned;
using Steamworks;

namespace OpenMod.Unturned.API
{
    public interface IUnturnedPlayerActor : ICommandActor
    {
        public Player Player { get; }
        public SteamPlayer SteamPlayer { get; }
        public CSteamID SteamId { get; }
    }
}
