using System;
using System.Threading.Tasks;
using OpenMod.Extensions.Games.Abstractions.Acl;
using OpenMod.Extensions.Games.Abstractions.Players;
using OpenMod.Unturned.Entities;
using SDG.Unturned;

namespace OpenMod.Unturned.Building
{
    public class UnturnedBuildableOwnership : IOwnership
    {
        public string OwnerPlayerId { get; }

        public string OwnerGroupId { get; }

        public UnturnedBuildableOwnership(BarricadeData barricade)
        {
            OwnerPlayerId = barricade.owner.ToString();
            OwnerGroupId = barricade.group.ToString();
        }

        public UnturnedBuildableOwnership(StructureData structure)
        {
            OwnerPlayerId = structure.owner.ToString();
            OwnerGroupId = structure.group.ToString();
        }

        public Task<bool> HasAccessAsync(IPlayer player)
        {
            if (!(player is UnturnedPlayer uPlayer))
            {
                return Task.FromResult(false);
            }

            if (OwnerPlayerId.Equals(uPlayer.SteamId.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(true);
            }

            if (OwnerGroupId.Equals(uPlayer.SteamPlayer.playerID.group.ToString(), StringComparison.OrdinalIgnoreCase) ||
                OwnerGroupId.Equals(uPlayer.Player.quests.groupID.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
    }
}