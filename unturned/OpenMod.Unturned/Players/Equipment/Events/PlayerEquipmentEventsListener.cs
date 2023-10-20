using System;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using OpenMod.Unturned.Events;
using OpenMod.Unturned.Items;
using OpenMod.Unturned.Patching;
using SDG.Unturned;

namespace OpenMod.Unturned.Players.Equipment.Events
{
    [UsedImplicitly]
    internal class PlayerEquipmentEventsListener : UnturnedPlayerEventsListener
    {
        public PlayerEquipmentEventsListener(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            SubscribePlayer(static player => ref player.equipment.onEquipRequested, player => OnEquipRequested);
            SubscribePlayer(static player => ref player.equipment.onDequipRequested, player => OnDequipRequested);
        }

        public override void Subscribe()
        {
            OnItemEquipped += Events_OnItemEquipped;
            OnItemUnequipped += Events_OnItemUnequipped;
            PlayerEquipment.OnPunch_Global += Events_OnPunchGlobal;
        }

        public override void Unsubscribe()
        {
            OnItemEquipped -= Events_OnItemEquipped;
            OnItemUnequipped -= Events_OnItemUnequipped;
            PlayerEquipment.OnPunch_Global -= Events_OnPunchGlobal;
        }

        private void Events_OnItemEquipped(Player nativePlayer)
        {
            var player = GetUnturnedPlayer(nativePlayer)!;

            var page = nativePlayer.equipment.equippedPage;

            var index = nativePlayer.inventory.getIndex(page,
                nativePlayer.equipment.equipped_x, nativePlayer.equipment.equipped_y);
            if (index == byte.MaxValue)
            {
                return;
            }

            var item = nativePlayer.inventory.getItem(page, index);
            if (item == null)
            {
                return;
            }

            var inventoryItem =
                new UnturnedInventoryItem(player!.Inventory, item);

            var @event = new UnturnedPlayerItemEquippedEvent(player, inventoryItem.Item);
            Emit(@event);
        }

        private void Events_OnItemUnequipped(Player nativePlayer)
        {
            var player = GetUnturnedPlayer(nativePlayer)!;

            var @event = new UnturnedPlayerItemUnequippedEvent(player);

            Emit(@event);
        }

        private void Events_OnPunchGlobal(PlayerEquipment equipment, EPlayerPunch punch)
        {
            var player = GetUnturnedPlayer(equipment.player)!;

            var @event = new UnturnedPlayerPunchEvent(player, punch);

            Emit(@event);
        }

        private void OnEquipRequested(PlayerEquipment equipment, ItemJar jar, ItemAsset asset, ref bool shouldAllow)
        {
            var player = GetUnturnedPlayer(equipment.player)!;

            var inventoryItem = new UnturnedInventoryItem(player.Inventory, jar);

            var @event = new UnturnedPlayerItemEquippingEvent(player, inventoryItem.Item)
            {
                IsCancelled = !shouldAllow
            };

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
        }

        private void OnDequipRequested(PlayerEquipment equipment, ref bool shouldAllow)
        {
            var player = GetUnturnedPlayer(equipment.player)!;

            var inv = player.Player.inventory;

            var page = equipment.equippedPage;

            var index = inv.getIndex(page, equipment.equipped_x, equipment.equipped_y);

            var jar = inv.getItem(page, index);

            if (jar?.item == null)
                return;

            var inventoryItem = new UnturnedInventoryItem(player.Inventory, jar);

            var @event = new UnturnedPlayerItemUnequippingEvent(player, inventoryItem.Item)
            {
                IsCancelled = !shouldAllow
            };

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
        }

        private delegate void ItemEquipped(Player player);

        private static event ItemEquipped? OnItemEquipped;

        private delegate void ItemUnequipped(Player player);

        private static event ItemUnequipped? OnItemUnequipped;

        [UsedImplicitly]
        [HarmonyPatch]
        private static class Patches
        {
            [HarmonyCleanup]
            public static Exception? Cleanup(Exception ex, MethodBase original)
            {
                HarmonyExceptionHandler.ReportCleanupException(typeof(Patches), ex, original);
                return null;
            }

            // ReSharper disable InconsistentNaming
            [UsedImplicitly]
            [HarmonyPatch(typeof(PlayerEquipment), nameof(PlayerEquipment.ReceiveEquip))]
            [HarmonyPrefix]
            public static void PreTellEquip(PlayerEquipment __instance, out ushort __state)
            {
                __state = __instance.itemID;
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(PlayerEquipment), nameof(PlayerEquipment.ReceiveEquip))]
            [HarmonyPostfix]
            public static void PostTellEquip(PlayerEquipment __instance, ushort __state)
            {
                if (__state == 0 && __instance.itemID == 0)
                    return;

                if (__state != 0)
                {
                    OnItemUnequipped?.Invoke(__instance.player);
                }

                if (__instance.useable != null)
                {
                    OnItemEquipped?.Invoke(__instance.player);
                }
            }
            // ReSharper restore InconsistentNaming
        }
    }
}