using Cysharp.Threading.Tasks;
using HarmonyLib;
using OpenMod.Extensions.Games.Abstractions.Items;
using OpenMod.UnityEngine.Extensions;
using SDG.NetTransport;
using SDG.Unturned;
using System;
using System.Numerics;
using System.Threading.Tasks;

namespace OpenMod.Unturned.Items
{
    public class UnturnedItemDrop : IItemDrop
    {
        private static readonly ClientStaticMethod<byte, byte, uint> s_SendTakeItem;

        static UnturnedItemDrop()
        {
            s_SendTakeItem = AccessTools.StaticFieldRefAccess<ItemManager, ClientStaticMethod<byte, byte, uint>>("SendTakeItem");
        }

        private readonly ItemData m_ItemData;

        public IItem Item { get; }

        public Vector3 Position { get; }

        public byte RegionX { get; }

        public byte RegionY { get; }

        public ItemRegion Region { get; }

        public UnturnedItemDrop(ItemData itemData)
        {
            m_ItemData = itemData;
            Item = new UnturnedItem(itemData.item, DestroyAsync);
            Position = itemData.point.ToSystemVector();

            GetRegion(itemData, out var x, out var y);
            RegionX = x;
            RegionY = y;

            Region = ItemManager.regions[RegionX, RegionY];
        }

        public UnturnedItemDrop(byte regionX, byte regionY, ItemData itemData)
        {
            m_ItemData = itemData;
            Item = new UnturnedItem(itemData.item, DestroyAsync);
            Position = itemData.point.ToSystemVector();

            RegionX = regionX;
            RegionY = regionY;
            Region = ItemManager.regions[RegionX, RegionY];
        }

        private static void GetRegion(ItemData itemData, out byte x, out byte y)
        {
            if (!Regions.tryGetCoordinate(itemData.point, out x, out y))
            {
                throw new Exception($"Failed to get region for point {itemData.point}");
            }
        }

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

                s_SendTakeItem.Invoke(ENetReliability.Reliable,
                    Regions.EnumerateClients(RegionX, RegionY, ItemManager.ITEM_REGIONS),
                    RegionX, RegionY, m_ItemData.instanceID);

                return true;
            }

            return DestroyTask().AsTask();
        }
    }
}