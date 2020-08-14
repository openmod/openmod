namespace OpenMod.Extensions.Games.Abstractions.Items
{
    public interface IItemAsset
    {
        string ItemAssetId { get; }
        string ItemName { get; }
        string ItemType { get; }
    }
}