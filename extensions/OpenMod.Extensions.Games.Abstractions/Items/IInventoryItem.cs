using System.Threading.Tasks;

namespace OpenMod.Extensions.Games.Abstractions.Items
{
    public interface IInventoryItem : IItemInstance
    {
        Task DropAsync();

        Task DestroyAsync();
    }
}