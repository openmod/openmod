using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;

namespace OpenMod.Core.Patching
{
    public static class HarmonyExtensions
    {
        private static readonly HarmonyMethod s_NopMethod;

        static HarmonyExtensions()
        {
            s_NopMethod = new HarmonyMethod(typeof(HarmonyExtensions).GetMethod(nameof(Nop), BindingFlags.Static | BindingFlags.NonPublic));
        }

        /// <summary>
        /// Only applies patches that satisfy any of the given conditions.
        /// </summary>
        public static void PatchAllConditional(this Harmony harmony, Assembly asm, params string[] conditions)
        {
            var conditionsSet = new HashSet<string>(conditions, StringComparer.OrdinalIgnoreCase);

            AccessTools.GetTypesFromAssembly(asm)
                .Do(type =>
                {
                    var typeConditions = type.GetCustomAttributes<HarmonyConditionalPatchAttribute>()
                        .Select(d => d.Condition)
                        .ToList();

                    if (!typeConditions.Any() || !typeConditions.Any(d => conditionsSet.Contains(d)))
                    {
                        return;
                    }

                    harmony.CreateClassProcessor(type).Patch();
                });
        }

        /// <summary>
        /// Applies a no-operation patch.
        /// </summary>
        public static void NopPatch(this Harmony harmony, MethodBase method)
        {
            harmony.Patch(method, prefix: s_NopMethod);
        }

        private static bool Nop()
        {
            return false;
        }
    }
}