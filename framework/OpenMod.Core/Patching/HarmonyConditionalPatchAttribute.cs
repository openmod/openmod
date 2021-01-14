using System;
using System.Globalization;

namespace OpenMod.Core.Patching
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Delegate | AttributeTargets.Method, AllowMultiple = true)]
    public class HarmonyConditionalPatchAttribute : Attribute
    {
        public string Condition { get; }

        public HarmonyConditionalPatchAttribute(string condition)
        {
            Condition = condition;
        }
    }
}