extern alias JetBrainsAnnotations;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using HarmonyLib;
using JetBrainsAnnotations::JetBrains.Annotations;
using OpenMod.API.Eventing;
using OpenMod.Unturned.Events;
using OpenMod.Unturned.Items;
using OpenMod.Unturned.Patching;
using SDG.Unturned;

namespace OpenMod.Unturned.Players.Clothing.Events
{
    [UsedImplicitly]
    internal class PlayerClothingEventsListener : UnturnedEventsListener
    {
        public PlayerClothingEventsListener(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override void Subscribe()
        {
            OnWearBackpack += Events_OnWearBackpack;
            OnWearGlasses += Events_OnWearGlasses;
            OnWearHat += Events_OnWearHat;
            OnWearMask += Events_OnWearMask;
            OnWearPants += Events_OnWearPants;
            OnWearShirt += Events_OnWearShirt;
            OnWearVest += Events_OnWearVest;

            PlayerClothing.OnBackpackChanged_Global += PlayerClothing_OnBackpackChanged_Global;
            PlayerClothing.OnGlassesChanged_Global += PlayerClothing_OnGlassesChanged_Global;
            PlayerClothing.OnHatChanged_Global += PlayerClothing_OnHatChanged_Global;
            PlayerClothing.OnMaskChanged_Global += PlayerClothing_OnMaskChanged_Global;
            PlayerClothing.OnPantsChanged_Global += PlayerClothing_OnPantsChanged_Global;
            PlayerClothing.OnShirtChanged_Global += PlayerClothing_OnShirtChanged_Global;
            PlayerClothing.OnVestChanged_Global += PlayerClothing_OnVestChanged_Global;
        }

        public override void Unsubscribe()
        {
            OnWearBackpack -= Events_OnWearBackpack;
            OnWearGlasses -= Events_OnWearGlasses;
            OnWearHat -= Events_OnWearHat;
            OnWearMask -= Events_OnWearMask;
            OnWearPants -= Events_OnWearPants;
            OnWearShirt -= Events_OnWearShirt;
            OnWearVest -= Events_OnWearVest;

            PlayerClothing.OnBackpackChanged_Global -= PlayerClothing_OnBackpackChanged_Global;
            PlayerClothing.OnGlassesChanged_Global -= PlayerClothing_OnGlassesChanged_Global;
            PlayerClothing.OnHatChanged_Global -= PlayerClothing_OnHatChanged_Global;
            PlayerClothing.OnMaskChanged_Global -= PlayerClothing_OnMaskChanged_Global;
            PlayerClothing.OnPantsChanged_Global -= PlayerClothing_OnPantsChanged_Global;
            PlayerClothing.OnShirtChanged_Global -= PlayerClothing_OnShirtChanged_Global;
            PlayerClothing.OnVestChanged_Global -= PlayerClothing_OnVestChanged_Global;
        }

        private void Events_OnWearBackpack(Player nativePlayer, ItemClothingAsset asset, byte quality, byte[] state, ref bool cancel)
        {
            var player = GetUnturnedPlayer(nativePlayer)!;

            ICancellableEvent @event;

            if (asset is not ItemBackpackAsset backpackAsset)
            {
                var c = nativePlayer.clothing;

                var item = new Item(c.backpack, 1, c.backpackQuality, c.backpackState);
                @event = new UnturnedPlayerClothingUnequippingEvent(player, new UnturnedClothingItem(item, player, ClothingType.Backpack));
            }
            else
            {
                var item = new Item(backpackAsset.id, 1, quality, state);
                @event = new UnturnedPlayerClothingEquippingEvent(player, new UnturnedClothingItem(item, player, ClothingType.Backpack));
            }

            @event.IsCancelled = cancel;

            Emit(@event);

            cancel = @event.IsCancelled;
        }

        private void Events_OnWearGlasses(Player nativePlayer, ItemClothingAsset asset, byte quality, byte[] state, ref bool cancel)
        {
            var player = GetUnturnedPlayer(nativePlayer)!;

            ICancellableEvent @event;

            if (asset is not ItemGlassesAsset glassesAsset)
            {
                var c = nativePlayer.clothing;

                var item = new Item(c.glasses, 1, c.glassesQuality, c.glassesState);
                @event = new UnturnedPlayerClothingUnequippingEvent(player!, new UnturnedClothingItem(item, player, ClothingType.Glasses));
            }
            else
            {
                var item = new Item(glassesAsset.id, 1, quality, state);
                @event = new UnturnedPlayerClothingEquippingEvent(player!, new UnturnedClothingItem(item, player, ClothingType.Glasses));
            }

            @event.IsCancelled = cancel;

            Emit(@event);

            cancel = @event.IsCancelled;
        }

        private void Events_OnWearHat(Player nativePlayer, ItemClothingAsset asset, byte quality, byte[] state, ref bool cancel)
        {
            var player = GetUnturnedPlayer(nativePlayer)!;

            ICancellableEvent @event;

            if (asset is not ItemHatAsset hatAsset)
            {
                var c = nativePlayer.clothing;

                var item = new Item(c.hat, 1, c.hatQuality, c.hatState);
                @event = new UnturnedPlayerClothingUnequippingEvent(player!, new UnturnedClothingItem(item, player, ClothingType.Hat));
            }
            else
            {
                var item = new Item(hatAsset.id, 1, quality, state);
                @event = new UnturnedPlayerClothingEquippingEvent(player!, new UnturnedClothingItem(item, player, ClothingType.Hat));
            }

            @event.IsCancelled = cancel;

            Emit(@event);

            cancel = @event.IsCancelled;
        }

        private void Events_OnWearMask(Player nativePlayer, ItemClothingAsset asset, byte quality, byte[] state, ref bool cancel)
        {
            var player = GetUnturnedPlayer(nativePlayer)!;

            ICancellableEvent @event;

            if (asset is not ItemMaskAsset maskAsset)
            {
                var c = nativePlayer.clothing;

                var item = new Item(c.mask, 1, c.maskQuality, c.maskState);
                @event = new UnturnedPlayerClothingUnequippingEvent(player!, new UnturnedClothingItem(item, player, ClothingType.Mask));
            }
            else
            {
                var item = new Item(maskAsset.id, 1, quality, state);
                @event = new UnturnedPlayerClothingEquippingEvent(player!, new UnturnedClothingItem(item, player, ClothingType.Mask));
            }

            @event.IsCancelled = cancel;

            Emit(@event);

            cancel = @event.IsCancelled;
        }

        private void Events_OnWearPants(Player nativePlayer, ItemClothingAsset asset, byte quality, byte[] state, ref bool cancel)
        {
            var player = GetUnturnedPlayer(nativePlayer)!;

            ICancellableEvent @event;

            if (asset is not ItemPantsAsset pantsAsset)
            {
                var c = nativePlayer.clothing;
                var item = new Item(c.pants, 1, c.pantsQuality, c.pantsState);
                @event = new UnturnedPlayerClothingUnequippingEvent(player!, new UnturnedClothingItem(item, player, ClothingType.Pants));
            }
            else
            {
                var item = new Item(pantsAsset.id, 1, quality, state);
                @event = new UnturnedPlayerClothingEquippingEvent(player!, new UnturnedClothingItem(item, player, ClothingType.Pants));
            }

            @event.IsCancelled = cancel;

            Emit(@event);

            cancel = @event.IsCancelled;
        }

        private void Events_OnWearShirt(Player nativePlayer, ItemClothingAsset asset, byte quality, byte[] state, ref bool cancel)
        {
            var player = GetUnturnedPlayer(nativePlayer)!;

            ICancellableEvent @event;

            if (asset is not ItemShirtAsset shirtAsset)
            {
                var c = nativePlayer.clothing;

                var item = new Item(c.shirt, 1, c.shirtQuality, c.shirtState);
                @event = new UnturnedPlayerClothingUnequippingEvent(player!, new UnturnedClothingItem(item, player, ClothingType.Shirt));
            }
            else
            {
                var item = new Item(shirtAsset.id, 1, quality, state);
                @event = new UnturnedPlayerClothingEquippingEvent(player!, new UnturnedClothingItem(item, player, ClothingType.Shirt));
            }

            @event.IsCancelled = cancel;

            Emit(@event);

            cancel = @event.IsCancelled;
        }

        private void Events_OnWearVest(Player nativePlayer, ItemClothingAsset asset, byte quality, byte[] state, ref bool cancel)
        {
            var player = GetUnturnedPlayer(nativePlayer)!;

            ICancellableEvent @event;

            if (asset is not ItemVestAsset vestAsset)
            {
                var c = nativePlayer.clothing;

                var item = new Item(c.vest, 1, c.vestQuality, c.vestState);
                @event = new UnturnedPlayerClothingUnequippingEvent(player!, new UnturnedClothingItem(item, player, ClothingType.Vest));
            }
            else
            {
                var item = new Item(vestAsset.id, 1, quality, state);
                @event = new UnturnedPlayerClothingEquippingEvent(player!, new UnturnedClothingItem(item, player, ClothingType.Vest));
            }

            @event.IsCancelled = cancel;

            Emit(@event);

            cancel = @event.IsCancelled;
        }

        private void PlayerClothing_OnBackpackChanged_Global(PlayerClothing obj)
        {
            var player = GetUnturnedPlayer(obj.player)!;

            IEvent @event;

            if (obj.backpack == 0)
            {
                @event = new UnturnedPlayerClothingUnequippedEvent(player, ClothingType.Backpack);
            }
            else
            {
                var item = new Item(obj.backpack, 1, obj.backpackQuality, obj.backpackState);
                @event = new UnturnedPlayerClothingEquippedEvent(player, new UnturnedClothingItem(item, obj, ClothingType.Backpack));
            }

            Emit(@event);
        }

        private void PlayerClothing_OnVestChanged_Global(PlayerClothing obj)
        {
            var player = GetUnturnedPlayer(obj.player)!;

            IEvent @event;

            if (obj.vest == 0)
            {
                @event = new UnturnedPlayerClothingUnequippedEvent(player, ClothingType.Vest);
            }
            else
            {
                var item = new Item(obj.vest, 1, obj.vestQuality, obj.vestState);
                @event = new UnturnedPlayerClothingEquippedEvent(player, new UnturnedClothingItem(item, obj, ClothingType.Vest));
            }

            Emit(@event);
        }

        private void PlayerClothing_OnShirtChanged_Global(PlayerClothing obj)
        {
            var player = GetUnturnedPlayer(obj.player)!;

            IEvent @event;

            if (obj.shirt == 0)
            {
                @event = new UnturnedPlayerClothingUnequippedEvent(player, ClothingType.Shirt);
            }
            else
            {
                var item = new Item(obj.shirt, 1, obj.shirtQuality, obj.shirtState);
                @event = new UnturnedPlayerClothingEquippedEvent(player, new UnturnedClothingItem(item, obj, ClothingType.Shirt));
            }

            Emit(@event);
        }

        private void PlayerClothing_OnPantsChanged_Global(PlayerClothing obj)
        {
            var player = GetUnturnedPlayer(obj.player)!;

            IEvent @event;

            if (obj.pants == 0)
            {
                @event = new UnturnedPlayerClothingUnequippedEvent(player, ClothingType.Pants);
            }
            else
            {
                var item = new Item(obj.pants, 1, obj.pantsQuality, obj.pantsState);
                @event = new UnturnedPlayerClothingEquippedEvent(player, new UnturnedClothingItem(item, obj, ClothingType.Pants));
            }

            Emit(@event);
        }

        private void PlayerClothing_OnMaskChanged_Global(PlayerClothing obj)
        {
            var player = GetUnturnedPlayer(obj.player)!;

            IEvent @event;

            if (obj.mask == 0)
            {
                @event = new UnturnedPlayerClothingUnequippedEvent(player, ClothingType.Mask);
            }
            else
            {
                var item = new Item(obj.mask, 1, obj.maskQuality, obj.maskState);
                @event = new UnturnedPlayerClothingEquippedEvent(player, new UnturnedClothingItem(item, obj, ClothingType.Mask));
            }

            Emit(@event);
        }

        private void PlayerClothing_OnHatChanged_Global(PlayerClothing obj)
        {
            var player = GetUnturnedPlayer(obj.player)!;

            IEvent @event;

            if (obj.hat == 0)
            {
                @event = new UnturnedPlayerClothingUnequippedEvent(player, ClothingType.Hat);
            }
            else
            {
                var item = new Item(obj.hat, 1, obj.hatQuality, obj.hatState);
                @event = new UnturnedPlayerClothingEquippedEvent(player, new UnturnedClothingItem(item, obj, ClothingType.Hat));
            }

            Emit(@event);
        }

        private void PlayerClothing_OnGlassesChanged_Global(PlayerClothing obj)
        {
            var player = GetUnturnedPlayer(obj.player)!;

            IEvent @event;

            if (obj.glasses == 0)
            {
                @event = new UnturnedPlayerClothingUnequippedEvent(player, ClothingType.Glasses);
            }
            else
            {
                var item = new Item(obj.glasses, 1, obj.glassesQuality, obj.glassesState);
                @event = new UnturnedPlayerClothingEquippedEvent(player, new UnturnedClothingItem(item, obj, ClothingType.Glasses));
            }

            Emit(@event);
        }

        private delegate void WearClothing(Player player, ItemClothingAsset asset, byte quality, byte[] state, ref bool cancel);

        private static event WearClothing? OnWearBackpack;
        private static event WearClothing? OnWearGlasses;
        private static event WearClothing? OnWearHat;
        private static event WearClothing? OnWearMask;
        private static event WearClothing? OnWearPants;
        private static event WearClothing? OnWearShirt;
        private static event WearClothing? OnWearVest;

        [UsedImplicitly]
        [HarmonyPatch]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        internal static class Patches
        {
            [HarmonyCleanup]
            public static Exception? Cleanup(Exception ex, MethodBase original)
            {
                HarmonyExceptionHandler.ReportCleanupException(typeof(Patches), ex, original);
                return null;
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(PlayerClothing), nameof(PlayerClothing.askWearBackpack),
                typeof(ItemBackpackAsset), typeof(byte), typeof(byte[]), typeof(bool))]
            [HarmonyPrefix]
            public static bool AskWearBackpack(PlayerClothing __instance, ItemBackpackAsset asset, byte quality, byte[] state)
            {
                var cancel = false;

                OnWearBackpack?.Invoke(__instance.player, asset, quality, state, ref cancel);

                return !cancel;
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(PlayerClothing), nameof(PlayerClothing.askWearGlasses),
                typeof(ItemGlassesAsset), typeof(byte), typeof(byte[]), typeof(bool))]
            [HarmonyPrefix]
            public static bool AskWearGlasses(PlayerClothing __instance, ItemGlassesAsset asset, byte quality, byte[] state)
            {
                var cancel = false;

                OnWearGlasses?.Invoke(__instance.player, asset, quality, state, ref cancel);

                return !cancel;
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(PlayerClothing), nameof(PlayerClothing.askWearHat),
                typeof(ItemHatAsset), typeof(byte), typeof(byte[]), typeof(bool))]
            [HarmonyPrefix]
            public static bool AskWearHat(PlayerClothing __instance, ItemHatAsset asset, byte quality, byte[] state)
            {
                var cancel = false;

                OnWearHat?.Invoke(__instance.player, asset, quality, state, ref cancel);

                return !cancel;
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(PlayerClothing), nameof(PlayerClothing.askWearMask),
                typeof(ItemMaskAsset), typeof(byte), typeof(byte[]), typeof(bool))]
            [HarmonyPrefix]
            public static bool AskWearMask(PlayerClothing __instance, ItemMaskAsset asset, byte quality, byte[] state)
            {
                var cancel = false;

                OnWearMask?.Invoke(__instance.player, asset, quality, state, ref cancel);

                return !cancel;
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(PlayerClothing), nameof(PlayerClothing.askWearPants),
                typeof(ItemPantsAsset), typeof(byte), typeof(byte[]), typeof(bool))]
            [HarmonyPrefix]
            public static bool AskWearPants(PlayerClothing __instance, ItemPantsAsset asset, byte quality, byte[] state)
            {
                var cancel = false;

                OnWearPants?.Invoke(__instance.player, asset, quality, state, ref cancel);

                return !cancel;
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(PlayerClothing), nameof(PlayerClothing.askWearShirt),
                typeof(ItemShirtAsset), typeof(byte), typeof(byte[]), typeof(bool))]
            [HarmonyPrefix]
            public static bool AskWearShirt(PlayerClothing __instance, ItemShirtAsset asset, byte quality, byte[] state)
            {
                var cancel = false;

                OnWearShirt?.Invoke(__instance.player, asset, quality, state, ref cancel);

                return !cancel;
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(PlayerClothing), nameof(PlayerClothing.askWearVest),
                typeof(ItemVestAsset), typeof(byte), typeof(byte[]), typeof(bool))]
            [HarmonyPrefix]
            public static bool AskWearVest(PlayerClothing __instance, ItemVestAsset asset, byte quality, byte[] state)
            {
                var cancel = false;

                OnWearVest?.Invoke(__instance.player, asset, quality, state, ref cancel);

                return !cancel;
            }
        }
    }
}