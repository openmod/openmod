using System.Numerics;
using System.Threading.Tasks;
using OpenMod.Extensions.Games.Abstractions.Transforms;

namespace OpenMod.Extensions.Games.Abstractions.Items
{
    public interface IItemDrop : IItemInstance
    {
        public Vector3 Position { get; }

        Task<bool> DestroyAsync();
    }
}