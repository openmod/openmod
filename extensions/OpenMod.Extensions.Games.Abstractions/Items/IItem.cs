using System.Threading.Tasks;

namespace OpenMod.Extensions.Games.Abstractions.Items
{
    public interface IItem
    {
        string ItemInstanceId { get; }

        IItemAsset Asset { get; }

        IItemState State { get; }

        Task SetItemQualityAsync(double quality);

        Task SetItemAmountAsync(double amount);
    }
}