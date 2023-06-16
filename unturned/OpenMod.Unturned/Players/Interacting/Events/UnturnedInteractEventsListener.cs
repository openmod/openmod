extern alias JetBrainsAnnotations;
using System;
using System.Reflection;
using HarmonyLib;
using JetBrainsAnnotations::JetBrains.Annotations;
using OpenMod.Unturned.Events;
using OpenMod.Unturned.Patching;
using SDG.Unturned;
using Steamworks;

namespace OpenMod.Unturned.Players.Interacting.Events
{
    [UsedImplicitly]
    internal class UnturnedInteractEventsListener : UnturnedEventsListener
    {
        public UnturnedInteractEventsListener(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override void Subscribe()
        {
            OnBedClaiming += Events_OnBedBedClaiming;
        }

        public override void Unsubscribe()
        {
            OnBedClaiming -= Events_OnBedBedClaiming;
        }

        private void Events_OnBedBedClaiming(InteractableBed bed, CSteamID steamid, ref bool cancel)
        {
            var player = GetUnturnedPlayer(steamid);
            // Very important to make sure the player's not null because it has a delay on the server when the player unclaiming the bed
            // and it causes a null reference exception
            if (player != null)
            {
                var @event = new UnturnedPlayerBedClaimingEvent(bed, player)
                {
                    IsCancelled = cancel//by default cancel starts with false (check patch)
                };

                Emit(@event);
                cancel = @event.IsCancelled;
            }
        }

        private delegate void BedClaiming(InteractableBed bed, CSteamID steamID, ref bool cancel);
        private static event BedClaiming? OnBedClaiming;

        [UsedImplicitly]
        [HarmonyPatch]
        internal static class Patches
        {
            [HarmonyCleanup]
            public static Exception? Cleanup(Exception ex, MethodBase original)
            {
                HarmonyExceptionHandler.ReportCleanupException(typeof(Patches), ex, original);
                return null;
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(BarricadeManager), "ServerSetBedOwnerInternal")]
            [HarmonyPrefix]
            public static bool ServerSetBedOwnerInternal(InteractableBed bed, CSteamID steamID)
            {
                var cancel = false;

                OnBedClaiming?.Invoke(bed, steamID, ref cancel);

                return !cancel;
            }
        }
    }
}