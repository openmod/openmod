extern alias JetBrainsAnnotations;
using HarmonyLib;
using JetBrainsAnnotations::JetBrains.Annotations;
using OpenMod.Unturned.Events;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Reflection;

// ReSharper disable InconsistentNaming

namespace OpenMod.Unturned.Players.Skills.Events
{
    [UsedImplicitly]
    internal class PlayerSkillsEventsListener : UnturnedEventsListener
    {
        public PlayerSkillsEventsListener(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override void Subscribe()
        {
            OnExperienceUpdated += Events_OnExperienceUpdated;
        }

        public override void Unsubscribe()
        {
            OnExperienceUpdated -= Events_OnExperienceUpdated;
        }

        private void Events_OnExperienceUpdated(Player nativePlayer, uint experience)
        {
            var player = GetUnturnedPlayer(nativePlayer)!;
            var @event = new UnturnedPlayerExperienceUpdatedEvent(player, experience);

            Emit(@event);
        }

        private delegate void ExperienceUpdated(Player player, uint experience);

        private static event ExperienceUpdated? OnExperienceUpdated;

        [UsedImplicitly]
        [HarmonyPatch]
        internal static class ExperiencePatches
        {
            static IEnumerable<MethodBase> TargetMethods()
            {
                string[] methods =
                {
                    "askAward",
                    "askBoost",
                    "askPay",
                    "askPurchase",
                    "askSpend",
                    "askUpgrade",
                    "modXp",
                    "modXp2",
                    "onLifeUpdated"
                };

                foreach (var method in methods)
                {
                    yield return AccessTools.Method(typeof(PlayerSkills), method);
                }
            }

            [UsedImplicitly]
            static void Prefix(PlayerSkills __instance, out uint __state)
            {
                __state = __instance.experience;
            }

            [UsedImplicitly]
            static void Postfix(PlayerSkills __instance, uint __state)
            {
                if (__instance.experience != __state)
                {
                    OnExperienceUpdated?.Invoke(__instance.player, __instance.experience);
                }
            }
        }
    }
}