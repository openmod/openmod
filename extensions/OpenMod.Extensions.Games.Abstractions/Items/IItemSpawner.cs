using System.Numerics;
using System.Threading.Tasks;
using OpenMod.API.Ioc;

namespace OpenMod.Extensions.Games.Abstractions.Items
{
    [Service]
    public interface IItemSpawner
    {
        Task GiveItemAsync(IInventory inventory, string itemAssetId);

        Task DropItemAsync(Vector3 position, string itemAssetId);
    }
}