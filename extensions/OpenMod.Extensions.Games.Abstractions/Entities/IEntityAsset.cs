namespace OpenMod.Extensions.Games.Abstractions.Entities
{
    /// <summary>
    /// Represents an entity asset.
    /// </summary>
    public interface IEntityAsset
    {
        /// <summary>
        /// Gets the human-readable name of the asset.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the ID of the asset.
        /// </summary>
        string EntityAssetId { get; }

        /// <summary>
        /// Gets the type of the asset.
        /// </summary>
        string EntityType { get; }
    }
}