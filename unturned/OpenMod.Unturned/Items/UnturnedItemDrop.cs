using System;
using System.Numerics;
using System.Reflection;
using OpenMod.Extensions.Games.Abstractions.Items;
using SDG.Unturned;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using OpenMod.UnityEngine.Extensions;

namespace OpenMod.Unturned.Items
{
    public class UnturnedItemDrop : IItemDrop
    {
        private static readonly FieldInfo s_DespawnXField;
        private static readonly FieldInfo s_DespawnYField;
        private readonly ItemData m_ItemData;

        static UnturnedItemDrop()
        {
            s_DespawnXField = typeof(ItemManager).GetField("despawnItems_X", BindingFlags.Static | BindingFlags.NonPublic);
            s_DespawnYField = typeof(ItemManager).GetField("despawnItems_Y", BindingFlags.Static | BindingFlags.NonPublic);
        }

        public UnturnedItemDrop(ItemData itemData) : this(GetRegion(itemData), itemData)
        {

        }

        private static ItemRegion GetRegion(ItemData itemData)
        {
            if (!Regions.tryGetCoordinate(itemData.point, out var x, out var y))
            {
                throw new Exception($"Failed to get region for point {itemData.point}");
            }

            return ItemManager.regions[x, y];
        }

        public UnturnedItemDrop(ItemRegion region, ItemData itemData)
        {
            m_ItemData = itemData;
            Item = new UnturnedItem(itemData.item);
            Position = itemData.point.ToSystemVector();
            Region = region;
        }

        public IItem Item { get; }

        public Vector3 Position { get; }

        public ItemRegion Region { get; }

        public Task<bool> DestroyAsync()
        {
            async UniTask<bool> DestroyTask()
            {
                await UniTask.SwitchToMainThread();

                var count = Region.items.RemoveAll(d => ReferenceEquals(d, m_ItemData));
                if (count == 0)
                {
                    // already destroyed
                    return false;
                }

                var despawnX = (byte) s_DespawnXField.GetValue(null);
                var despawnY = (byte) s_DespawnYField.GetValue(null);
                ItemManager.instance.channel.send("tellTakeItem", ESteamCall.CLIENTS, despawnX, despawnY, ItemManager.ITEM_REGIONS, ESteamPacket.UPDATE_RELIABLE_BUFFER, despawnX, despawnY, m_ItemData.instanceID);
                return true;
            }

            return DestroyTask().AsTask();
        }
    }
}