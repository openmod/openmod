using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;

namespace OpenMod.Core.Patching
{
    public static class HarmonyExtensions
    {
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
    }
}