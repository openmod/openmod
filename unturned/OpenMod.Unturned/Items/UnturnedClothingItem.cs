using Cysharp.Threading.Tasks;
using HarmonyLib;
using OpenMod.Unturned.Players;
using OpenMod.Unturned.Players.Clothing;
using SDG.NetTransport;
using SDG.Unturned;
using System;
using System.Threading.Tasks;

namespace OpenMod.Unturned.Items
{
    public class UnturnedClothingItem : UnturnedItem
    {
        private static readonly ClientInstanceMethod<ushort, byte, byte[]> s_SendWearShirt =
            AccessTools.StaticFieldRefAccess<PlayerClothing, ClientInstanceMethod<ushort, byte, byte[]>>("SendWearShirt");

        private static readonly ClientInstanceMethod<ushort, byte, byte[]> s_SendWearPants =
            AccessTools.StaticFieldRefAccess<PlayerClothing, ClientInstanceMethod<ushort, byte, byte[]>>("SendWearPants");

        private static readonly ClientInstanceMethod<ushort, byte, byte[]> s_SendWearHat =
            AccessTools.StaticFieldRefAccess<PlayerClothing, ClientInstanceMethod<ushort, byte, byte[]>>("SendWearHat");

        private static readonly ClientInstanceMethod<ushort, byte, byte[]> s_SendWearBackpack =
            AccessTools.StaticFieldRefAccess<PlayerClothing, ClientInstanceMethod<ushort, byte, byte[]>>("SendWearBackpack");

        private static readonly ClientInstanceMethod<ushort, byte, byte[]> s_SendWearVest =
            AccessTools.StaticFieldRefAccess<PlayerClothing, ClientInstanceMethod<ushort, byte, byte[]>>("SendWearVest");

        private static readonly ClientInstanceMethod<ushort, byte, byte[]> s_SendWearMask =
            AccessTools.StaticFieldRefAccess<PlayerClothing, ClientInstanceMethod<ushort, byte, byte[]>>("SendWearMask");

        private static readonly ClientInstanceMethod<ushort, byte, byte[]> s_SendWearGlasses =
            AccessTools.StaticFieldRefAccess<PlayerClothing, ClientInstanceMethod<ushort, byte, byte[]>>("SendWearGlasses");

        private static ClientInstanceMethod<ushort, byte, byte[]> GetInstanceMethod(ClothingType clothingType)
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

        private static Task<bool> DestroyClothingPieceAsync(UnturnedPlayer player, ClothingType clothingType)
        {
            async UniTask<bool> DestroyClothingPieceTask()
            {
                await UniTask.SwitchToMainThread();

                var method = GetInstanceMethod(clothingType);

                method.InvokeAndLoopback(player.Player.clothing.GetNetId(), ENetReliability.Reliable,
                    Provider.EnumerateClients_Remote(), 0, 0, new byte[0]);

                return true;
            }

            return DestroyClothingPieceTask().AsTask();
        }

        public ClothingType ClothingType { get; }

        public UnturnedClothingItem(Item item, UnturnedPlayer player, ClothingType clothingType) :
            base(item, () => DestroyClothingPieceAsync(player, clothingType))
        {
            ClothingType = clothingType;
        }
    }
}
