using System.Threading.Tasks;

namespace OpenMod.Extensions.Games.Abstractions.Items
{
    public interface IItem
    {
        bool IsEquipped { get; }

        string ItemAssetId { get; }

        string ItemInstanceId { get; }

        string ItemName { get; }

        string ItemType { get; }

        double ItemQuality { get; }
        
        double ItemAmount { get; }
        
        Task SetItemQualityAsync(double quality);

        Task SetItemAmountAsync(double amount);

        Task<bool> EquipAsync();

        Task<bool> UnequipAsync();

        Task<bool> DestroyAsync();

        Task<bool> DropAsync();
    }
}