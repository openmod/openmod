using System.Numerics;
using System.Threading.Tasks;
using JetBrains.Annotations;
using OpenMod.API.Ioc;

namespace OpenMod.Extensions.Games.Abstractions.Items
{
    [Service]
    public interface IItemSpawner
    {
        [CanBeNull]
        Task<IItemInstance> GiveItemAsync(IInventory inventory, string itemId, [CanBeNull] IItemState state = null);

        [CanBeNull]
        Task<IItemDrop> SpawnItemAsync(Vector3 position, string itemId, [CanBeNull] IItemState state = null);
    }
}