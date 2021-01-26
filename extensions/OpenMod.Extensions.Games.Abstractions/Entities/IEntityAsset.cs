namespace OpenMod.Extensions.Games.Abstractions.Entities
{
    /// <summary>
    /// Represents an entity asset.
    /// </summary>
    public interface IEntityAsset
    {
        /// <value>
        /// The human-readable name of the asset.
        /// </value>
        string Name { get; }

        /// <value>
        /// The ID of the asset.
        /// </value>
        string EntityAssetId { get; }

        /// <value>
        /// The type of the asset.
        /// </value>
        string EntityType { get; }
    }
}