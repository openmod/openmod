using Cysharp.Threading.Tasks;
using HarmonyLib;
using OpenMod.Unturned.Players.Clothing;
using SDG.NetTransport;
using SDG.Unturned;
using System;
using System.Threading.Tasks;
using OpenMod.Extensions.Games.Abstractions.Items;
using OpenMod.Unturned.Players;

namespace OpenMod.Unturned.Items
{
    public class UnturnedClothingItem : IItemInstance
    {
        private static readonly ClientInstanceMethod<Guid, byte, byte[]> s_SendWearShirt =
            AccessTools.StaticFieldRefAccess<PlayerClothing, ClientInstanceMethod<Guid, byte, byte[]>>("SendWearShirt");

        private static readonly ClientInstanceMethod<Guid, byte, byte[]> s_SendWearPants =
            AccessTools.StaticFieldRefAccess<PlayerClothing, ClientInstanceMethod<Guid, byte, byte[]>>("SendWearPants");

        private static readonly ClientInstanceMethod<Guid, byte, byte[]> s_SendWearHat =
            AccessTools.StaticFieldRefAccess<PlayerClothing, ClientInstanceMethod<Guid, byte, byte[]>>("SendWearHat");

        private static readonly ClientInstanceMethod<Guid, byte, byte[]> s_SendWearBackpack =
            AccessTools.StaticFieldRefAccess<PlayerClothing, ClientInstanceMethod<Guid, byte, byte[]>>("SendWearBackpack");

        private static readonly ClientInstanceMethod<Guid, byte, byte[]> s_SendWearVest =
            AccessTools.StaticFieldRefAccess<PlayerClothing, ClientInstanceMethod<Guid, byte, byte[]>>("SendWearVest");

        private static readonly ClientInstanceMethod<Guid, byte, byte[]> s_SendWearMask =
            AccessTools.StaticFieldRefAccess<PlayerClothing, ClientInstanceMethod<Guid, byte, byte[]>>("SendWearMask");

        private static readonly ClientInstanceMethod<Guid, byte, byte[]> s_SendWearGlasses =
            AccessTools.StaticFieldRefAccess<PlayerClothing, ClientInstanceMethod<Guid, byte, byte[]>>("SendWearGlasses");

        private static ClientInstanceMethod<Guid, byte, byte[]> GetInstanceMethod(ClothingType clothingType)
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
                    Provider.EnumerateClients_Remote(), Guid.Empty, 0, new byte[0]);

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
