using System.Threading.Tasks;

namespace OpenMod.Extensions.Games.Abstractions.Items
{
    public interface IInventoryItem
    {
        IItem Item { get; }

        Task DropAsync();

        Task DestroyAsync();
    }
}