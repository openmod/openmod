using HarmonyLib;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using OpenMod.Core.Patching;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace OpenMod.Core.Configuration
{
    [HarmonyPatch]
    internal class FileConfigurationProviderPatch
    {
        [HarmonyCleanup]
        public static Exception? Cleanup(Exception? ex, MethodBase? original)
        {
            HarmonyExceptionHandler.ReportCleanupException(typeof(FileConfigurationProviderPatch), ex, original);
            return null;
        }

        private static readonly MethodInfo s_FileInfoExistsMethod = typeof(IFileInfo).GetProperty(nameof(IFileInfo.Exists)).GetGetMethod();
        private static readonly MethodInfo s_FileInfoLengthMethod = typeof(IFileInfo).GetProperty(nameof(IFileInfo.Length)).GetGetMethod();

        //if (file == null || !file.Exists) -> if (file == null || !file.Exists || file.Length == 0)
        [HarmonyPatch(typeof(FileConfigurationProvider), "Load", typeof(bool))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> GetSerializeMembersPatch(IEnumerable<CodeInstruction> insts, ILGenerator generator)
        {
            var instructions = new List<CodeInstruction>(insts);
            for (int i = 0; i < instructions.Count; i++)
            {
                var instruction = instructions[i];
                if (!instruction.Calls(s_FileInfoExistsMethod))
                {
                    continue;
                }

                var label = generator.DefineLabel();
                instructions.Insert(i + 1, new CodeInstruction(OpCodes.Brfalse_S, label));
                instructions.Insert(i + 2, new CodeInstruction(OpCodes.Ldloc_0));
                instructions.Insert(i + 3, new CodeInstruction(OpCodes.Callvirt, s_FileInfoLengthMethod));

                var brTrueInst = instructions[i + 4];
                brTrueInst.opcode = OpCodes.Brtrue_S;

                var ldArgInst = instructions[i + 5];
                ldArgInst.WithLabels(label);

                break;
            }

            return instructions;
        }
    }
}
