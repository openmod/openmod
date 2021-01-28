namespace OpenMod.Extensions.Games.Abstractions.Building
{
    /// <summary>
    /// Represents a buildable asset.
    /// </summary>
    public interface IBuildableAsset
    {
        /// <summary>
        /// Gets the ID of the asset.
        /// </summary>
        string BuildableAssetId { get; }

        /// <summary>
        /// Gets the type of the asset.
        /// </summary>
        string BuildableType { get; }
    }
}
