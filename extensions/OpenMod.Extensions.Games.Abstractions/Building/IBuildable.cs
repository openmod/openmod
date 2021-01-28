using System.Threading.Tasks;
using OpenMod.Extensions.Games.Abstractions.Acl;
using OpenMod.Extensions.Games.Abstractions.Entities;

namespace OpenMod.Extensions.Games.Abstractions.Building
{
    /// <summary>
    /// Represents a buildable object players can place.
    /// </summary>
    public interface IBuildable : IHasOwnership, IGameObject
    {
        /// <summary>
        /// Gets the asset of the object.
        /// </summary>
        IBuildableAsset Asset { get; }

        /// <summary>
        /// Gets the state of the object.
        /// </summary>
        IBuildableState State { get; }

        /// <summary>
        /// Gets the unique instance ID of the object.
        /// </summary>
        string BuildableInstanceId { get; }

        /// <summary>
        /// Destroys the object.
        /// </summary>
        Task DestroyAsync();
    }
}