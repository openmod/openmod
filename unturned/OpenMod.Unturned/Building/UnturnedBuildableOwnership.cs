using OpenMod.Extensions.Games.Abstractions.Acl;
using OpenMod.Extensions.Games.Abstractions.Players;
using OpenMod.Unturned.Players;
using SDG.Unturned;
using System;
using System.Threading.Tasks;

namespace OpenMod.Unturned.Building
{
    public class UnturnedBuildableOwnership : IOwnership
    {
        private readonly StructureData m_Structure;
        private readonly BarricadeData m_Barricade;

        public bool HasOwner
        {
            get
            {
                return !string.IsNullOrEmpty(OwnerPlayerId)
                       || !string.IsNullOrEmpty(OwnerGroupId);
            }
        }

        public string OwnerPlayerId
        {
            get
            {
                return m_Barricade != null
                    ? m_Barricade.owner.ToString()
                    : m_Structure.owner.ToString();
            }
        }

        public string OwnerGroupId
        {
            get
            {
                return m_Barricade != null
                    ? m_Barricade.@group.ToString()
                    : m_Structure.@group.ToString();
            }
        }

        public UnturnedBuildableOwnership(BarricadeData barricade)
        {
            m_Barricade = barricade;
        }

        public UnturnedBuildableOwnership(StructureData structure)
        {
            m_Structure = structure;
        }

        public Task<bool> HasAccessAsync(IPlayer player)
        {
            if (player is UnturnedPlayer unturnedPlayer)
            {
                if (OwnerPlayerId != null && string.Equals(OwnerPlayerId, unturnedPlayer.SteamId.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    return Task.FromResult(true);
                }

                if (OwnerGroupId != null
                    && (string.Equals(OwnerGroupId, unturnedPlayer.SteamPlayer.playerID.group.ToString(), StringComparison.OrdinalIgnoreCase)
                    || string.Equals(OwnerGroupId, unturnedPlayer.Player.quests.groupID.ToString(), StringComparison.OrdinalIgnoreCase)))
                {
                    return Task.FromResult(true);
                }
            }

            return Task.FromResult(false);
        }
    }
}