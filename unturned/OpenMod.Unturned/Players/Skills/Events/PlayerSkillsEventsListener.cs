using HarmonyLib;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Users;
using OpenMod.Unturned.Events;
using SDG.Unturned;
using System.Collections.Generic;
using System.Reflection;

namespace OpenMod.Unturned.Players.Skills.Events
{
    internal class PlayerSkillsEventsListener : UnturnedEventsListener
    {
        public PlayerSkillsEventsListener(IOpenModHost openModHost,
            IEventBus eventBus,
            IUserManager userManager) : base(openModHost, eventBus, userManager)
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
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerExperienceUpdatedEvent @event = new UnturnedPlayerExperienceUpdatedEvent(player, experience);

            Emit(@event);
        }

        private delegate void ExperienceUpdated(Player player, uint experience);

        private static event ExperienceUpdated OnExperienceUpdated;

        [HarmonyPatch]
        private class ExperiencePatches
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

                foreach (string method in methods)
                {
                    yield return AccessTools.Method(typeof(PlayerSkills), method);
                }
            }

            static void Prefix(PlayerSkills __instance, out uint __state)
            {
                __state = __instance.experience;
            }

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