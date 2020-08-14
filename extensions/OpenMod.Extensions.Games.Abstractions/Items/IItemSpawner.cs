using System.Numerics;
using System.Threading.Tasks;
using JetBrains.Annotations;
using OpenMod.API.Ioc;

namespace OpenMod.Extensions.Games.Abstractions.Items
{
    [Service]
    public interface IItemSpawner
    {
        Task<IItem> GiveItemAsync(IInventory inventory, string itemId, [CanBeNull] IItemState state = null);

        Task<IItem> SpawnItemAsync(Vector3 position, string itemId, [CanBeNull] IItemState state = null);
    }
}