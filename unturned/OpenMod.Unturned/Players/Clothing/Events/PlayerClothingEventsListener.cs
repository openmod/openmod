using HarmonyLib;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Users;
using OpenMod.Unturned.Events;
using OpenMod.Unturned.Items;
using SDG.Unturned;
// ReSharper disable DelegateSubtraction

namespace OpenMod.Unturned.Players.Clothing.Events
{
    internal class PlayerClothingEventsListener : UnturnedPlayerEventsListener
    {
        public PlayerClothingEventsListener(IOpenModHost openModHost,
            IEventBus eventBus,
            IUserManager userManager) : base(openModHost, eventBus, userManager)
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
        }

        public override void SubscribePlayer(Player player)
        {
            player.clothing.onBackpackUpdated += (a, b, c) => OnBackpackUpdated(player, a, b, c);
            player.clothing.onGlassesUpdated += (a, b, c) => OnGlassesUpdated(player, a, b, c);
            player.clothing.onHatUpdated += (a, b, c) => OnHatUpdated(player, a, b, c);
            player.clothing.onMaskUpdated += (a, b, c) => OnMaskUpdated(player, a, b, c);
            player.clothing.onPantsUpdated += (a, b, c) => OnPantsUpdated(player, a, b, c);
            player.clothing.onShirtUpdated += (a, b, c) => OnShirtUpdated(player, a, b, c);
            player.clothing.onVestUpdated += (a, b, c) => OnVestUpdated(player, a, b, c);
        }

        public override void UnsubscribePlayer(Player player)
        {
            player.clothing.onBackpackUpdated -= (a, b, c) => OnBackpackUpdated(player, a, b, c);
            player.clothing.onGlassesUpdated -= (a, b, c) => OnGlassesUpdated(player, a, b, c);
            player.clothing.onHatUpdated -= (a, b, c) => OnHatUpdated(player, a, b, c);
            player.clothing.onMaskUpdated -= (a, b, c) => OnMaskUpdated(player, a, b, c);
            player.clothing.onPantsUpdated -= (a, b, c) => OnPantsUpdated(player, a, b, c);
            player.clothing.onShirtUpdated -= (a, b, c) => OnShirtUpdated(player, a, b, c);
            player.clothing.onVestUpdated -= (a, b, c) => OnVestUpdated(player, a, b, c);
        }

        private void Events_OnWearBackpack(Player nativePlayer, ushort id, byte quality, byte[] state, ref bool cancel)
        {
            var player = GetUnturnedPlayer(nativePlayer);

            ICancellableEvent @event;

            if (id == 0)
            {
                var c = nativePlayer.clothing;

                var item = new Item(c.backpack, 1, c.backpackQuality, c.backpackState);

                @event = new UnturnedPlayerClothingUnequippingEvent(player, new UnturnedItem(item), ClothingType.Backpack);
            }
            else
            {
                var item = new Item(id, 1, quality, state);

                @event = new UnturnedPlayerClothingEquippingEvent(player, new UnturnedItem(item), ClothingType.Backpack);
            }

            @event.IsCancelled = cancel;

            Emit(@event);

            cancel = @event.IsCancelled;
        }

        private void Events_OnWearGlasses(Player nativePlayer, ushort id, byte quality, byte[] state, ref bool cancel)
        {
            var player = GetUnturnedPlayer(nativePlayer);

            ICancellableEvent @event;

            if (id == 0)
            {
                PlayerClothing c = nativePlayer.clothing;

                Item item = new Item(c.glasses, 1, c.glassesQuality, c.glassesState);

                @event = new UnturnedPlayerClothingUnequippingEvent(player, new UnturnedItem(item), ClothingType.Glasses);
            }
            else
            {
                Item item = new Item(id, 1, quality, state);

                @event = new UnturnedPlayerClothingEquippingEvent(player, new UnturnedItem(item), ClothingType.Glasses);
            }

            @event.IsCancelled = cancel;

            Emit(@event);

            cancel = @event.IsCancelled;
        }

        private void Events_OnWearHat(Player nativePlayer, ushort id, byte quality, byte[] state, ref bool cancel)
        {
            var player = GetUnturnedPlayer(nativePlayer);

            ICancellableEvent @event;

            if (id == 0)
            {
                PlayerClothing c = nativePlayer.clothing;

                Item item = new Item(c.hat, 1, c.hatQuality, c.hatState);

                @event = new UnturnedPlayerClothingUnequippingEvent(player, new UnturnedItem(item), ClothingType.Hat);
            }
            else
            {
                Item item = new Item(id, 1, quality, state);

                @event = new UnturnedPlayerClothingEquippingEvent(player, new UnturnedItem(item), ClothingType.Hat);
            }

            @event.IsCancelled = cancel;

            Emit(@event);

            cancel = @event.IsCancelled;
        }

        private void Events_OnWearMask(Player nativePlayer, ushort id, byte quality, byte[] state, ref bool cancel)
        {
            var player = GetUnturnedPlayer(nativePlayer);

            ICancellableEvent @event;

            if (id == 0)
            {
                PlayerClothing c = nativePlayer.clothing;

                Item item = new Item(c.mask, 1, c.maskQuality, c.maskState);

                @event = new UnturnedPlayerClothingUnequippingEvent(player, new UnturnedItem(item), ClothingType.Mask);
            }
            else
            {
                Item item = new Item(id, 1, quality, state);

                @event = new UnturnedPlayerClothingEquippingEvent(player, new UnturnedItem(item), ClothingType.Mask);
            }

            @event.IsCancelled = cancel;

            Emit(@event);

            cancel = @event.IsCancelled;
        }

        private void Events_OnWearPants(Player nativePlayer, ushort id, byte quality, byte[] state, ref bool cancel)
        {
            var player = GetUnturnedPlayer(nativePlayer);

            ICancellableEvent @event;

            if (id == 0)
            {
                PlayerClothing c = nativePlayer.clothing;

                Item item = new Item(c.pants, 1, c.pantsQuality, c.pantsState);

                @event = new UnturnedPlayerClothingUnequippingEvent(player, new UnturnedItem(item), ClothingType.Pants);
            }
            else
            {
                Item item = new Item(id, 1, quality, state);

                @event = new UnturnedPlayerClothingEquippingEvent(player, new UnturnedItem(item), ClothingType.Pants);
            }

            @event.IsCancelled = cancel;

            Emit(@event);

            cancel = @event.IsCancelled;
        }

        private void Events_OnWearShirt(Player nativePlayer, ushort id, byte quality, byte[] state, ref bool cancel)
        {
            var player = GetUnturnedPlayer(nativePlayer);

            ICancellableEvent @event;

            if (id == 0)
            {
                PlayerClothing c = nativePlayer.clothing;

                Item item = new Item(c.shirt, 1, c.shirtQuality, c.shirtState);

                @event = new UnturnedPlayerClothingUnequippingEvent(player, new UnturnedItem(item), ClothingType.Shirt);
            }
            else
            {
                Item item = new Item(id, 1, quality, state);

                @event = new UnturnedPlayerClothingEquippingEvent(player, new UnturnedItem(item), ClothingType.Shirt);
            }

            @event.IsCancelled = cancel;

            Emit(@event);

            cancel = @event.IsCancelled;
        }

        private void Events_OnWearVest(Player nativePlayer, ushort id, byte quality, byte[] state, ref bool cancel)
        {
            var player = GetUnturnedPlayer(nativePlayer);

            ICancellableEvent @event;

            if (id == 0)
            {
                PlayerClothing c = nativePlayer.clothing;

                Item item = new Item(c.vest, 1, c.vestQuality, c.vestState);

                @event = new UnturnedPlayerClothingUnequippingEvent(player, new UnturnedItem(item), ClothingType.Vest);
            }
            else
            {
                Item item = new Item(id, 1, quality, state);

                @event = new UnturnedPlayerClothingEquippingEvent(player, new UnturnedItem(item), ClothingType.Vest);
            }

            @event.IsCancelled = cancel;

            Emit(@event);

            cancel = @event.IsCancelled;
        }

        private void OnBackpackUpdated(Player nativePlayer, ushort id, byte quality, byte[] state)
        {
            var player = GetUnturnedPlayer(nativePlayer);

            IEvent @event;

            if (id == 0)
            {
                @event = new UnturnedPlayerClothingUnequippedEvent(player, ClothingType.Backpack);
            }
            else
            {
                Item item = new Item(id, 1, quality, state);

                @event = new UnturnedPlayerClothingEquippedEvent(player, new UnturnedItem(item), ClothingType.Backpack);
            }

            Emit(@event);
        }

        private void OnGlassesUpdated(Player nativePlayer, ushort id, byte quality, byte[] state)
        {
            var player = GetUnturnedPlayer(nativePlayer);

            IEvent @event;

            if (id == 0)
            {
                @event = new UnturnedPlayerClothingUnequippedEvent(player, ClothingType.Glasses);
            }
            else
            {
                Item item = new Item(id, 1, quality, state);

                @event = new UnturnedPlayerClothingEquippedEvent(player, new UnturnedItem(item), ClothingType.Glasses);
            }

            Emit(@event);
        }

        private void OnHatUpdated(Player nativePlayer, ushort id, byte quality, byte[] state)
        {
            var player = GetUnturnedPlayer(nativePlayer);

            IEvent @event;

            if (id == 0)
            {
                @event = new UnturnedPlayerClothingUnequippedEvent(player, ClothingType.Hat);
            }
            else
            {
                Item item = new Item(id, 1, quality, state);

                @event = new UnturnedPlayerClothingEquippedEvent(player, new UnturnedItem(item), ClothingType.Hat);
            }

            Emit(@event);
        }

        private void OnMaskUpdated(Player nativePlayer, ushort id, byte quality, byte[] state)
        {
            var player = GetUnturnedPlayer(nativePlayer);

            IEvent @event;

            if (id == 0)
            {
                @event = new UnturnedPlayerClothingUnequippedEvent(player, ClothingType.Mask);
            }
            else
            {
                Item item = new Item(id, 1, quality, state);

                @event = new UnturnedPlayerClothingEquippedEvent(player, new UnturnedItem(item), ClothingType.Mask);
            }

            Emit(@event);
        }

        private void OnPantsUpdated(Player nativePlayer, ushort id, byte quality, byte[] state)
        {
            var player = GetUnturnedPlayer(nativePlayer);

            IEvent @event;

            if (id == 0)
            {
                @event = new UnturnedPlayerClothingUnequippedEvent(player, ClothingType.Pants);
            }
            else
            {
                Item item = new Item(id, 1, quality, state);

                @event = new UnturnedPlayerClothingEquippedEvent(player, new UnturnedItem(item), ClothingType.Pants);
            }

            Emit(@event);
        }

        private void OnShirtUpdated(Player nativePlayer, ushort id, byte quality, byte[] state)
        {
            var player = GetUnturnedPlayer(nativePlayer);

            IEvent @event;

            if (id == 0)
            {
                @event = new UnturnedPlayerClothingUnequippedEvent(player, ClothingType.Shirt);
            }
            else
            {
                Item item = new Item(id, 1, quality, state);

                @event = new UnturnedPlayerClothingEquippedEvent(player, new UnturnedItem(item), ClothingType.Shirt);
            }

            Emit(@event);
        }

        private void OnVestUpdated(Player nativePlayer, ushort id, byte quality, byte[] state)
        {
            var player = GetUnturnedPlayer(nativePlayer);

            IEvent @event;

            if (id == 0)
            {
                @event = new UnturnedPlayerClothingUnequippedEvent(player, ClothingType.Vest);
            }
            else
            {
                Item item = new Item(id, 1, quality, state);

                @event = new UnturnedPlayerClothingEquippedEvent(player, new UnturnedItem(item), ClothingType.Vest);
            }

            Emit(@event);
        }

        private delegate void WearClothing(Player player, ushort id, byte quality, byte[] state, ref bool cancel);

        private static event WearClothing OnWearBackpack;
        private static event WearClothing OnWearGlasses;
        private static event WearClothing OnWearHat;
        private static event WearClothing OnWearMask;
        private static event WearClothing OnWearPants;
        private static event WearClothing OnWearShirt;
        private static event WearClothing OnWearVest;

        [HarmonyPatch]
        private class Patches
        {
            [HarmonyPatch(typeof(PlayerClothing), "askWearBackpack")]
            [HarmonyPrefix]
            private static bool AskWearBackpack(PlayerClothing __instance, ushort id, byte quality, byte[] state)
            {
                var cancel = false;

                OnWearBackpack?.Invoke(__instance.player, id, quality, state, ref cancel);

                return !cancel;
            }

            [HarmonyPatch(typeof(PlayerClothing), "askWearGlasses")]
            [HarmonyPrefix]
            private static bool AskWearGlasses(PlayerClothing __instance, ushort id, byte quality, byte[] state)
            {
                var cancel = false;

                OnWearGlasses?.Invoke(__instance.player, id, quality, state, ref cancel);

                return !cancel;
            }

            [HarmonyPatch(typeof(PlayerClothing), "askWearHat")]
            [HarmonyPrefix]
            private static bool AskWearHat(PlayerClothing __instance, ushort id, byte quality, byte[] state)
            {
                var cancel = false;

                OnWearHat?.Invoke(__instance.player, id, quality, state, ref cancel);

                return !cancel;
            }

            [HarmonyPatch(typeof(PlayerClothing), "askWearMask")]
            [HarmonyPrefix]
            private static bool AskWearMask(PlayerClothing __instance, ushort id, byte quality, byte[] state)
            {
                var cancel = false;

                OnWearMask?.Invoke(__instance.player, id, quality, state, ref cancel);

                return !cancel;
            }

            [HarmonyPatch(typeof(PlayerClothing), "askWearPants")]
            [HarmonyPrefix]
            private static bool AskWearPants(PlayerClothing __instance, ushort id, byte quality, byte[] state)
            {
                var cancel = false;

                OnWearPants?.Invoke(__instance.player, id, quality, state, ref cancel);

                return !cancel;
            }

            [HarmonyPatch(typeof(PlayerClothing), "askWearShirt")]
            [HarmonyPrefix]
            private static bool AskWearShirt(PlayerClothing __instance, ushort id, byte quality, byte[] state)
            {
                var cancel = false;

                OnWearShirt?.Invoke(__instance.player, id, quality, state, ref cancel);

                return !cancel;
            }

            [HarmonyPatch(typeof(PlayerClothing), "askWearVest")]
            [HarmonyPrefix]
            private static bool AskWearVest(PlayerClothing __instance, ushort id, byte quality, byte[] state)
            {
                var cancel = false;

                OnWearVest?.Invoke(__instance.player, id, quality, state, ref cancel);

                return !cancel;
            }
        }
    }
}