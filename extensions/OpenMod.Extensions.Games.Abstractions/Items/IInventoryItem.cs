using System.Threading.Tasks;

namespace OpenMod.Extensions.Games.Abstractions.Items
{
    public interface IInventoryItem : IItemObject
    {
        Task DropAsync();

        Task DestroyAsync();
    }
}