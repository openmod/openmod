using JetBrains.Annotations;

namespace OpenMod.Extensions.Games.Abstractions.Building
{
    /// <summary>
    /// Represents a buildable asset.
    /// </summary>
    public interface IBuildableAsset
    {
        /// <value>
        /// The ID of the asset. Cannot be null.
        /// </value>
        [NotNull]
        string BuildableAssetId { get; }

        /// <value>
        /// The type of the asset. Cannot be null.
        /// </value>
        [NotNull]
        string BuildableType { get; }
    }
}
