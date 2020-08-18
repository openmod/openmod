using System.Threading.Tasks;
using OpenMod.Extensions.Games.Abstractions.Players;

namespace OpenMod.Extensions.Games.Abstractions.Buildings
{
    public interface IOwnership
    {
        string OwnerPlayerId { get; }

        string AssociatedGroupId { get; }

        Task<bool> HasAccess(IPlayer player);
    }
}