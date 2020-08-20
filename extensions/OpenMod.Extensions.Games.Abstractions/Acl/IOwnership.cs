using System.Threading.Tasks;
using OpenMod.Extensions.Games.Abstractions.Players;

namespace OpenMod.Extensions.Games.Abstractions.Acl
{
    public interface IOwnership
    {
        string OwnerPlayerId { get; }

        string OwnerGroupId { get; }

        Task<bool> HasAccessAsync(IPlayer player);
    }
}