using System.Threading.Tasks;
using JetBrains.Annotations;
using OpenMod.Extensions.Games.Abstractions.Players;

namespace OpenMod.Extensions.Games.Abstractions.Acl
{
    public interface IOwnership
    {
        bool HasOwner { get; }

        [CanBeNull]
        string OwnerPlayerId { get; }

        [CanBeNull]
        string OwnerGroupId { get; }

        Task<bool> HasAccessAsync(IPlayer player);
    }
}