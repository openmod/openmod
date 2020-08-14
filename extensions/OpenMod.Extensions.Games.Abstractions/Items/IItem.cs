using System.Threading.Tasks;

namespace OpenMod.Extensions.Games.Abstractions.Items
{
    public interface IItem
    {
        bool IsEquipped { get; }

        string ItemInstanceId { get; }

        IItemAsset Asset { get; }

        IItemState State { get; }

        Task SetItemQualityAsync(double quality);

        Task SetItemAmountAsync(double amount);

        Task<bool> EquipAsync();

        Task<bool> UnequipAsync();

        Task<bool> DestroyAsync();

        Task<bool> DropAsync();
    }
}