namespace OpenMod.Extensions.Games.Abstractions.Entities
{
    public interface IEntityAsset
    {
        string Name { get; }

        string EntityAssetId { get; }

        string EntityType { get; }
    }
}