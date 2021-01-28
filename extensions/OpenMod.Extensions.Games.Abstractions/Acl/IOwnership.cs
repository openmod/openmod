using System.Threading.Tasks;
using OpenMod.Extensions.Games.Abstractions.Players;

namespace OpenMod.Extensions.Games.Abstractions.Acl
{
    /// <summary>
    /// Represts the ownership of an object.
    /// </summary>
    public interface IOwnership
    {
        /// <summary>Checks if the object has an owner. Either <see cref="OwnerPlayerId"/> or <see cref="OwnerGroupId"/> will not be null if true.</summary>
        /// <returns>
        /// <b>True</b> if the object has an owner; otherwise, <b>false</b>.
        /// </returns>
        bool HasOwner { get; }

        /// <summary>
        /// Gets the ID of the player owning this object. Returns null if no player owns the object.
        /// </summary>
        string? OwnerPlayerId { get; }

        /// <summary>
        /// Gets the ID of the group owning this object. Returns null if no group owns the object.
        /// </summary>
        string? OwnerGroupId { get; }

        /// <summary>
        /// Checks if the given player has access to this object.
        /// </summary>
        /// <param name="player">The player to check against.</param>
        /// <returns><b>True</b> if the player has access; otherwise, <b>false.</b></returns>
        Task<bool> HasAccessAsync(IPlayer player);
    }
}