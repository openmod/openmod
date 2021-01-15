using System;

namespace OpenMod.Core.Patching
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class HarmonyConditionalPatchAttribute : Attribute
    {
        public string Condition { get; }

        public HarmonyConditionalPatchAttribute(string condition)
        {
            Condition = condition;
        }
    }
}