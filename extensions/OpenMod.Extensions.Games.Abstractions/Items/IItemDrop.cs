using System.Threading.Tasks;
using OpenMod.Extensions.Games.Abstractions.Transforms;

namespace OpenMod.Extensions.Games.Abstractions.Items
{
    public interface IItemDrop
    {
        public IItem Item { get; }

        public IWorldTransform Transform { get; }

        Task<bool> DestroyAsync();
    }
}