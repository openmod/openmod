using JetBrains.Annotations;

namespace OpenMod.Extensions.Games.Abstractions.Entities
{
    /// <summary>
    /// Represents an entity asset.
    /// </summary>
    public interface IEntityAsset
    {
        /// <value>
        /// The human-readable name of the asset. Cannot be null.
        /// </value>
        [NotNull]
        string Name { get; }

        /// <value>
        /// The ID of the asset. Cannot be null.
        /// </value>
        [NotNull]
        string EntityAssetId { get; }

        /// <value>
        /// The type of the asset. Cannot be null.
        /// </value>
        [NotNull]
        string EntityType { get; }
    }
}