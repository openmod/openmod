using HarmonyLib;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Users;
using OpenMod.Unturned.Entities;
using SDG.Unturned;

namespace OpenMod.Unturned.Events.Players.Clothing
{
    internal class PlayerClothingEventsListener : UnturnedEventsListener
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

        private void Events_OnWearBackpack(Player nativePlayer, ushort id, byte quality, byte[] state, out bool cancel)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            ICancellableEvent @event;

            if (id == 0)
            {
                PlayerClothing c = nativePlayer.clothing;

                Item item = new Item(c.backpack, 1, c.backpackQuality, c.backpackState);

                @event = new UnturnedPlayerBackpackDequipEvent(player, item);
            }
            else
            {
                Item item = new Item(id, 1, quality, state);

                @event = new UnturnedPlayerBackpackEquipEvent(player, item);
            }

            Emit(@event);

            cancel = @event.IsCancelled;
        }

        private void Events_OnWearGlasses(Player nativePlayer, ushort id, byte quality, byte[] state, out bool cancel)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            ICancellableEvent @event;

            if (id == 0)
            {
                PlayerClothing c = nativePlayer.clothing;

                Item item = new Item(c.glasses, 1, c.glassesQuality, c.glassesState);

                @event = new UnturnedPlayerGlassesDequipEvent(player, item);
            }
            else
            {
                Item item = new Item(id, 1, quality, state);

                @event = new UnturnedPlayerGlassesEquipEvent(player, item);
            }

            Emit(@event);

            cancel = @event.IsCancelled;
        }

        private void Events_OnWearHat(Player nativePlayer, ushort id, byte quality, byte[] state, out bool cancel)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            ICancellableEvent @event;

            if (id == 0)
            {
                PlayerClothing c = nativePlayer.clothing;

                Item item = new Item(c.hat, 1, c.hatQuality, c.hatState);

                @event = new UnturnedPlayerHatDequipEvent(player, item);
            }
            else
            {
                Item item = new Item(id, 1, quality, state);

                @event = new UnturnedPlayerHatEquipEvent(player, item);
            }

            Emit(@event);

            cancel = @event.IsCancelled;
        }

        private void Events_OnWearMask(Player nativePlayer, ushort id, byte quality, byte[] state, out bool cancel)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            ICancellableEvent @event;

            if (id == 0)
            {
                PlayerClothing c = nativePlayer.clothing;

                Item item = new Item(c.mask, 1, c.maskQuality, c.maskState);

                @event = new UnturnedPlayerMaskDequipEvent(player, item);
            }
            else
            {
                Item item = new Item(id, 1, quality, state);

                @event = new UnturnedPlayerMaskEquipEvent(player, item);
            }

            Emit(@event);

            cancel = @event.IsCancelled;
        }

        private void Events_OnWearPants(Player nativePlayer, ushort id, byte quality, byte[] state, out bool cancel)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            ICancellableEvent @event;

            if (id == 0)
            {
                PlayerClothing c = nativePlayer.clothing;

                Item item = new Item(c.pants, 1, c.pantsQuality, c.pantsState);

                @event = new UnturnedPlayerPantsDequipEvent(player, item);
            }
            else
            {
                Item item = new Item(id, 1, quality, state);

                @event = new UnturnedPlayerPantsEquipEvent(player, item);
            }

            Emit(@event);

            cancel = @event.IsCancelled;
        }

        private void Events_OnWearShirt(Player nativePlayer, ushort id, byte quality, byte[] state, out bool cancel)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            ICancellableEvent @event;

            if (id == 0)
            {
                PlayerClothing c = nativePlayer.clothing;

                Item item = new Item(c.shirt, 1, c.shirtQuality, c.shirtState);

                @event = new UnturnedPlayerShirtDequipEvent(player, item);
            }
            else
            {
                Item item = new Item(id, 1, quality, state);

                @event = new UnturnedPlayerShirtEquipEvent(player, item);
            }

            Emit(@event);

            cancel = @event.IsCancelled;
        }

        private void Events_OnWearVest(Player nativePlayer, ushort id, byte quality, byte[] state, out bool cancel)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            ICancellableEvent @event;

            if (id == 0)
            {
                PlayerClothing c = nativePlayer.clothing;

                Item item = new Item(c.vest, 1, c.vestQuality, c.vestState);

                @event = new UnturnedPlayerVestDequipEvent(player, item);
            }
            else
            {
                Item item = new Item(id, 1, quality, state);

                @event = new UnturnedPlayerVestEquipEvent(player, item);
            }

            Emit(@event);

            cancel = @event.IsCancelled;
        }

        private delegate void WearClothing(Player player, ushort id, byte quality, byte[] state, out bool cancel);

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
            [HarmonyPostfix]
            private static bool AskWearBackpack(PlayerClothing __instance, ushort id, byte quality, byte[] state)
            {
                bool cancel = false;

                OnWearBackpack?.Invoke(__instance.player, id, quality, state, out cancel);

                return !cancel;
            }

            [HarmonyPatch(typeof(PlayerClothing), "askWearGlasses")]
            [HarmonyPostfix]
            private static bool AskWearGlasses(PlayerClothing __instance, ushort id, byte quality, byte[] state)
            {
                bool cancel = false;

                OnWearGlasses?.Invoke(__instance.player, id, quality, state, out cancel);

                return !cancel;
            }

            [HarmonyPatch(typeof(PlayerClothing), "askWearHat")]
            [HarmonyPostfix]
            private static bool AskWearHat(PlayerClothing __instance, ushort id, byte quality, byte[] state)
            {
                bool cancel = false;

                OnWearHat?.Invoke(__instance.player, id, quality, state, out cancel);

                return !cancel;
            }

            [HarmonyPatch(typeof(PlayerClothing), "askWearMask")]
            [HarmonyPostfix]
            private static bool AskWearMask(PlayerClothing __instance, ushort id, byte quality, byte[] state)
            {
                bool cancel = false;

                OnWearMask?.Invoke(__instance.player, id, quality, state, out cancel);

                return !cancel;
            }

            [HarmonyPatch(typeof(PlayerClothing), "askWearPants")]
            [HarmonyPostfix]
            private static bool AskWearPants(PlayerClothing __instance, ushort id, byte quality, byte[] state)
            {
                bool cancel = false;

                OnWearPants?.Invoke(__instance.player, id, quality, state, out cancel);

                return !cancel;
            }

            [HarmonyPatch(typeof(PlayerClothing), "askWearShirt")]
            [HarmonyPostfix]
            private static bool AskWearShirt(PlayerClothing __instance, ushort id, byte quality, byte[] state)
            {
                bool cancel = false;

                OnWearShirt?.Invoke(__instance.player, id, quality, state, out cancel);

                return !cancel;
            }

            [HarmonyPatch(typeof(PlayerClothing), "askWearVest")]
            [HarmonyPostfix]
            private static bool AskWearVest(PlayerClothing __instance, ushort id, byte quality, byte[] state)
            {
                bool cancel = false;

                OnWearVest?.Invoke(__instance.player, id, quality, state, out cancel);

                return !cancel;
            }
        }
    }
}