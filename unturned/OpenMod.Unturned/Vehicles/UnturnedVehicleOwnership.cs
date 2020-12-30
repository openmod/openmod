using OpenMod.Extensions.Games.Abstractions.Acl;
using OpenMod.Extensions.Games.Abstractions.Players;
using OpenMod.Unturned.Players;
using SDG.Unturned;
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
                return m_Vehicle.isLocked && (PlayerId != 0 || GroupId != 0);
            }
        }

        public string OwnerPlayerId
        {
            get { return PlayerId.ToString(); }
        }

        public string OwnerGroupId
        {
            get { return GroupId.ToString(); }
        }

        private ulong PlayerId
        {
            get { return m_Vehicle.lockedOwner.m_SteamID; }
        }

        private ulong GroupId
        {
            get { return m_Vehicle.lockedGroup.m_SteamID; }
        }

        public UnturnedVehicleOwnership(InteractableVehicle vehicle)
        {
            m_Vehicle = vehicle;
        }

        public Task<bool> HasAccessAsync(IPlayer player)
        {
            if (player is not UnturnedPlayer unturnedPlayer)
                return Task.FromResult(false);

            if (!HasOwner)
                return Task.FromResult(true);

            //Explanation at UnturnedBuildableOwnership
            if (PlayerId == unturnedPlayer.SteamId.m_SteamID)
            {
                return Task.FromResult(true);
            }

            return Task.FromResult(GroupId != 0 && GroupId == unturnedPlayer.Player.quests.groupID.m_SteamID);
        }
    }
}