using OpenMod.Extensions.Games.Abstractions.Acl;
using OpenMod.Extensions.Games.Abstractions.Players;
using OpenMod.Unturned.Players;
using SDG.Unturned;
using System;
using System.Threading.Tasks;

namespace OpenMod.Unturned.Vehicles
{
    public class UnturnedVehicleOwnership : IOwnership
    {
        private readonly InteractableVehicle m_Vehicle;

        public bool HasOwner
        {
            get
            {
                return !string.IsNullOrEmpty(OwnerPlayerId)
                       || !string.IsNullOrEmpty(OwnerGroupId);
            }
        }

        public string OwnerPlayerId => m_Vehicle.lockedOwner.ToString();

        public string OwnerGroupId => m_Vehicle.lockedGroup.ToString();

        public UnturnedVehicleOwnership(InteractableVehicle vehicle)
        {
            m_Vehicle = vehicle;
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