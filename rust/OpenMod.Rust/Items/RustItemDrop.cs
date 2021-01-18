using System.Numerics;
using System.Threading.Tasks;
using OpenMod.Extensions.Games.Abstractions.Entities;
using OpenMod.Extensions.Games.Abstractions.Items;
using OpenMod.Extensions.Games.Abstractions.Transforms;
using OpenMod.Rust.Transforms;

namespace OpenMod.Rust.Items
{
    public class RustItemDrop : IItemDrop, IGameObject
    {
        public RustItem Item { get; }

        public Vector3 Position
        {
            get { return Transform.Position; }
        }

        public DroppedItem DroppedItem { get; }

        public RustItemDrop(DroppedItem droppedItem)
        {
            Item = new RustItem(droppedItem.item);
            DroppedItem = droppedItem;
            Transform = new RustNetworkableTransform(droppedItem);
        }

        IItem IItemInstance.Item
        {
            get { return Item; }
        }

        public IWorldTransform Transform { get; }

        public Task<bool> DestroyAsync()
        {
            if (DroppedItem.IsDestroyed)
            {
                return Task.FromResult(false);
            }

            DroppedItem.Kill();
            return Task.FromResult(true);
        }
    }
}
