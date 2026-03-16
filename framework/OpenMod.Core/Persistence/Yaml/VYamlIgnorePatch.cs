using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Xml.Serialization;
using HarmonyLib;
using MonoMod.Utils;
using OpenMod.API.Persistence;
using OpenMod.Core.Patching;
using OpenMod.NuGet;
using VYaml.Annotations;
using VYaml.Emitter;
using VYaml.Parser;
using VYaml.Serialization;

namespace OpenMod.Core.Persistence.Yaml
{
    [HarmonyPatch]
    internal static class VYamlIgnorePatch
    {
        #region CleanUp
        [HarmonyCleanup]
        public static Exception? Cleanup(Exception? ex, MethodBase? original)
        {
            HarmonyExceptionHandler.ReportCleanupException(typeof(VYamlIgnorePatch), ex, original);
            return null;
        }
        #endregion


        #region GetTypes
        private static readonly MethodInfo s_GetFormatterMethod = typeof(GeneratedResolver).GetMethod(nameof(GeneratedResolver.GetFormatter));
        private static Type GetFormatterTypeFromObjType(this Type type)
        {
            return type
                .GetNestedTypes()
                .FirstOrDefault(tp => typeof(IYamlFormatter<>)
                .IsAssignableFrom(tp))
                ??
                s_GetFormatterMethod
                .MakeGenericMethod(type)
                .Invoke(GeneratedResolver.Instance, null)
                .GetType();
        }

        private static Type GetObjTypeFromMethod(this MethodBase originalMethod)
        {
            if (originalMethod is MethodInfo methodInfo)
            {
                var returnType = methodInfo.ReturnType;
                if (returnType != typeof(void))
                {
                    return returnType;
                }

                var parameters = methodInfo.GetParameters();
                if (parameters.Length == 3 && parameters[0].ParameterType.GetElementType() == typeof(Utf8YamlEmitter) && parameters[2].ParameterType == typeof(YamlSerializationContext))
                {
                    return parameters[1].ParameterType;
                }
            }

            return originalMethod.GetFormatterTypeFromMethod().GetObjTypeFromFormatterType();
        }

        private static Type GetFormatterTypeFromMethod(this MemberInfo originalMethod)
        {
            return originalMethod.GetRealDeclaringType() ?? originalMethod.ReflectedType;
        }

        private static Type GetObjTypeFromFormatterType(this Type formatterType)
        {
            var formatterInterfaceType = formatterType.GetInterfaces().First(i => i.IsConstructedGenericType && i.GetGenericTypeDefinition() == typeof(IYamlFormatter<>));
            return formatterInterfaceType.GenericTypeArguments.First();
        }

        private static IEnumerable<MemberInfo> GetIgnoredMembersInfo(this Type type)
        {
            return type
                .GetMembers(BindingFlags.Instance | BindingFlags.Public)
                .Where(m =>
                {
                    if (m is PropertyInfo p)
                    {
                        if (!p.CanRead && p.CanWrite)
                        {
                            return false;
                        }
                    }
                    else if (m is not FieldInfo)
                    {
                        return true;
                    }

                    return Attribute.IsDefined(m, typeof(SerializeIgnoreAttribute));
                });
        }
        /*
         * This can be used to create a second fallback if needed
         * private static readonly BindingFlags s_CacheFlags = BindingFlags.Static | BindingFlags.NonPublic;
         * private static readonly Type s_CacheType = typeof(GeneratedResolver).GetNestedType("Cache`1", s_CacheFlags);
         * s_CacheType.MakeGenericType(type).GetField("Formatter", s_CacheType).GetValue(null);
         */

        private static IEnumerable<FieldInfo> GetFieldsInstToIgnore(this MemberInfo originalMethod)
        {
            var formatterType = originalMethod.GetFormatterTypeFromMethod();
            var objType = formatterType.GetObjTypeFromFormatterType();


            var membersToIgnore = objType
                .GetIgnoredMembersInfo()
                .Select(m => $"{m.Name}KeyUtf8Bytes");
            return formatterType
                .GetFields(BindingFlags.Static | BindingFlags.NonPublic)
                .Where(f => membersToIgnore.Any(m => m.Equals(f.Name)))
                .ToList();
        }
        #endregion


        #region RegisterFormatterPatch
        private static readonly Harmony s_Harmony = new("OpenMod.Core");

        private static readonly MethodInfo s_SerializePatchMethod = AccessTools.Method(typeof(VYamlIgnorePatch), nameof(YamlFormatterSerializePatch));
        private static readonly MethodInfo s_DeserializePatchMethod = AccessTools.Method(typeof(VYamlIgnorePatch), nameof(YamlFormatterDeserializePatch));

        /*
         * when [YamlObject] is registered we will check if have SerializeIgnoreAttribute
         * if it has with need to patch it with transpiler to ignore that field/prop
         * the problem is target class is dynamic and the parent is a generic interface
         * also this patch should run only 1 time per class
         */
        [HarmonyPatch(typeof(GeneratedResolver), "TryInvokeRegisterYamlFormatter")]
        [HarmonyPostfix]
        private static void TryRegisterYamlFormatterPrePatch(in bool __result, Type type)
        {
            if (!__result || !type.GetIgnoredMembersInfo().Any())
            {
                return;
            }

            var formatterType = type.GetFormatterTypeFromObjType();

            var serializeMethod = formatterType.GetMethod("Serialize");
            s_Harmony.Patch(serializeMethod, transpiler: s_SerializePatchMethod);

            var deserializeMethod = formatterType.GetMethod("Deserialize");
            s_Harmony.Patch(deserializeMethod, transpiler: s_DeserializePatchMethod);
        }
        #endregion


        #region SerizalizePatch
        private delegate string KeyNameMutator(string str, NamingConvention namingConvention);
        private static readonly KeyNameMutator s_KeyNameMutatorDelegate = AccessTools.MethodDelegate<KeyNameMutator>("VYaml.Internal.KeyNameMutator:Mutate");

        private static IEnumerable<CodeInstruction> YamlFormatterSerializePatch(IEnumerable<CodeInstruction> insts, MethodBase originalMethod)
        {
            var objType = originalMethod.GetObjTypeFromMethod();
            var objNamingConvention = objType.GetCustomAttribute<YamlObjectAttribute>().NamingConvention;
            var membersToIgnore = objType
                .GetIgnoredMembersInfo()
                .Select(m => s_KeyNameMutatorDelegate(m.Name, objNamingConvention)).ToList();

            //12 Start instructions that we dont need to check
            //3 End instructions that we dont need to check
            if (insts.Count() <= 12 + 3/* || !membersToIgnore.Any()*/)
            {
                //nothing to patch
                return insts;
            }

            var instructions = new List<CodeInstruction>(insts);
            int maxCount()
            {
                return instructions.Count - 10;
            }

            //We start to check fields at index 13
            for (var i = 13; i < maxCount(); i++)
            {
                var instruction = instructions[i];
                if (!membersToIgnore.Any(instruction.LoadsConstant))
                {
                    continue;
                }

                instructions.RemoveRange(i, 9);
            }
            return instructions;
        }
        #endregion


        #region DeserializePatch
        private static readonly MethodInfo s_SkipNodeMethod = typeof(YamlParser).GetMethod(nameof(YamlParser.SkipCurrentNode));
        private static IEnumerable<CodeInstruction> YamlFormatterDeserializePatch(IEnumerable<CodeInstruction> insts, MethodBase originalMethod)
        {
            //42 Start instructions that we dont need to check
            //26 End instructions that we dont need to check
            var fieldsToIgnore = originalMethod.GetFieldsInstToIgnore();
            if (insts.Count() <= 42 + 26 || !fieldsToIgnore.Any())
            {
                //nothing to patch
                return insts;
            }

            var instructions = new List<CodeInstruction>(insts);
            int maxCount()
            {
                return instructions.Count - 26;
            }

            //We start to check fields at index 40
            //but it really just start at index 41
            for (var i = 40; i < maxCount(); i++)
            {
                var instruction = instructions[i];
                if (!fieldsToIgnore.Any(f => instruction.LoadsField(f)))
                {
                    continue;
                }

                //Skipping first 6 they are just getting key
                //The first instruction is pop from getting key
                //we capture it just to make sure
                var popInstruction = instructions[i + 6];
                if (popInstruction.opcode != OpCodes.Pop)
                {
                    continue;
                }

                //Jump instructions means end of block to override
                //It should be on these two indexes
                byte countToRemove;
                var jumpInstruction = instructions[i + 11];
                if (!IsJump(jumpInstruction))
                {
                    jumpInstruction = instructions[i + 12];
                    if (!IsJump(jumpInstruction))
                    {
                        break;
                    }

                    countToRemove = 5;
                }
                else
                {
                    countToRemove = 4;
                }

                var workingIndex = i + 7;
                instructions.RemoveRange(workingIndex, countToRemove);
                instructions.InsertRange(workingIndex, [new CodeInstruction(OpCodes.Ldarg_1), new CodeInstruction(OpCodes.Call, s_SkipNodeMethod)]);
            }
            return instructions;

            static bool IsJump(CodeInstruction inst)
            {
                return inst.opcode == OpCodes.Br || inst.opcode == OpCodes.Br_S;
            }
        }
        #endregion
    }
}