namespace OpenMod.Extensions.Games.Abstractions.Building
{
    public interface IBuildableAsset
    {
        string BuildableAssetId { get; }

        string BuildableType { get; }
    }
}
