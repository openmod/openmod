using System.Threading.Tasks;
using OpenMod.Extensions.Games.Abstractions.Buildings;
using OpenMod.Extensions.Games.Abstractions.Players;
using OpenMod.Unturned.Entities;
using SDG.Unturned;

namespace OpenMod.Unturned.Buildings
{
    public class UnturnedBuildableOwnership : IOwnership
    {
        public string OwnerPlayerId { get; }

        public string AssociatedGroupId { get; }

        public UnturnedBuildableOwnership(BarricadeData barricade)
        {
            OwnerPlayerId = barricade.owner.ToString();
            AssociatedGroupId = barricade.group.ToString();
        }

        public UnturnedBuildableOwnership(StructureData structure)
        {
            OwnerPlayerId = structure.owner.ToString();
            AssociatedGroupId = structure.group.ToString();
        }

        public Task<bool> HasAccess(IPlayer player)
        {
            if (!(player is UnturnedPlayer uPlayer))
                return Task.FromResult(result: false);

            if (OwnerPlayerId.Equals(uPlayer.SteamId))
                return Task.FromResult(result: true);

            return Task.FromResult(AssociatedGroupId.Equals(uPlayer.SteamPlayer.playerID.group.ToString()) ||
                                   AssociatedGroupId.Equals(uPlayer.Player.quests.groupID));

            // 1 liner:
            //return Task.FromResult(player is UnturnedPlayer uPlayer && (OwnerPlayerId.Equals(uPlayer.SteamId) || AssociatedGroupId.Equals(uPlayer.SteamPlayer.playerID.group.ToString()) || AssociatedGroupId.Equals(uPlayer.Player.quests.groupID)));
        }
    }
}