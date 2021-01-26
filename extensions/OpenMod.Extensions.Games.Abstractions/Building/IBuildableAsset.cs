namespace OpenMod.Extensions.Games.Abstractions.Building
{
    /// <summary>
    /// Represents a buildable asset.
    /// </summary>
    public interface IBuildableAsset
    {
        /// <value>
        /// The ID of the asset.
        /// </value>
        string BuildableAssetId { get; }

        /// <value>
        /// The type of the asset.
        /// </value>
        string BuildableType { get; }
    }
}
