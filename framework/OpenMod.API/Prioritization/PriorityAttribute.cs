using System;

namespace OpenMod.API.Prioritization
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class PriorityAttribute : Attribute
    {
        public Priority Priority { get; set; } = Priority.Normal;
    }
}