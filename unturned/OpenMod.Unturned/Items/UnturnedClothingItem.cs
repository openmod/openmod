using Cysharp.Threading.Tasks;
using HarmonyLib;
using OpenMod.Extensions.Games.Abstractions.Items;
using OpenMod.Unturned.Players;
using OpenMod.Unturned.Players.Clothing;
using SDG.NetTransport;
using SDG.Unturned;
using System;
using System.Threading.Tasks;

namespace OpenMod.Unturned.Items
{
    public class UnturnedClothingItem : IItemInstance
    {
        private static readonly ClientInstanceMethod<Guid, byte, byte[], bool> s_SendWearShirt =
            AccessTools.StaticFieldRefAccess<PlayerClothing, ClientInstanceMethod<Guid, byte, byte[], bool>>("SendWearShirt");

        private static readonly ClientInstanceMethod<Guid, byte, byte[], bool> s_SendWearPants =
            AccessTools.StaticFieldRefAccess<PlayerClothing, ClientInstanceMethod<Guid, byte, byte[], bool>>("SendWearPants");

        private static readonly ClientInstanceMethod<Guid, byte, byte[], bool> s_SendWearHat =
            AccessTools.StaticFieldRefAccess<PlayerClothing, ClientInstanceMethod<Guid, byte, byte[], bool>>("SendWearHat");

        private static readonly ClientInstanceMethod<Guid, byte, byte[], bool> s_SendWearBackpack =
            AccessTools.StaticFieldRefAccess<PlayerClothing, ClientInstanceMethod<Guid, byte, byte[], bool>>("SendWearBackpack");

        private static readonly ClientInstanceMethod<Guid, byte, byte[], bool> s_SendWearVest =
            AccessTools.StaticFieldRefAccess<PlayerClothing, ClientInstanceMethod<Guid, byte, byte[], bool>>("SendWearVest");

        private static readonly ClientInstanceMethod<Guid, byte, byte[], bool> s_SendWearMask =
            AccessTools.StaticFieldRefAccess<PlayerClothing, ClientInstanceMethod<Guid, byte, byte[], bool>>("SendWearMask");

        private static readonly ClientInstanceMethod<Guid, byte, byte[], bool> s_SendWearGlasses =
            AccessTools.StaticFieldRefAccess<PlayerClothing, ClientInstanceMethod<Guid, byte, byte[], bool>>("SendWearGlasses");

        private static ClientInstanceMethod<Guid, byte, byte[], bool> GetInstanceMethod(ClothingType clothingType)
        {
            return clothingType switch
            {
                ClothingType.Shirt => s_SendWearShirt,
                ClothingType.Pants => s_SendWearPants,
                ClothingType.Hat => s_SendWearHat,
                ClothingType.Backpack => s_SendWearBackpack,
                ClothingType.Vest => s_SendWearVest,
                ClothingType.Mask => s_SendWearMask,
                ClothingType.Glasses => s_SendWearGlasses,
                _ => throw new InvalidOperationException("Invalid clothing type")
            };
        }

        // ReSharper disable once SuggestBaseTypeForParameter
        private Task<bool> DestroyClothingPieceAsync(PlayerClothing playerClothing)
        {
            async UniTask<bool> DestroyClothingPieceTask()
            {
                await UniTask.SwitchToMainThread();

                var method = GetInstanceMethod(ClothingType);

                method.InvokeAndLoopback(playerClothing.GetNetId(), ENetReliability.Reliable,
                    Provider.GatherRemoteClientConnections(), Guid.Empty, 0, Array.Empty<byte>(), true);

                return true;
            }

            return DestroyClothingPieceTask().AsTask();
        }

        public ClothingType ClothingType { get; }

        public UnturnedClothingItem(Item item, UnturnedPlayer player, ClothingType clothingType)
        {
            ClothingType = clothingType;
            Item = new UnturnedItem(item, () => DestroyClothingPieceAsync(player.Player.clothing));
        }

        public UnturnedClothingItem(Item item, PlayerClothing playerClothing, ClothingType clothingType)
        {
            ClothingType = clothingType;
            Item = new UnturnedItem(item, () => DestroyClothingPieceAsync(playerClothing));
        }

        IItem IItemInstance.Item => Item;

        public UnturnedItem Item { get; }
    }
}
