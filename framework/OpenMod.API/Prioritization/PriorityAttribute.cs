using System;

namespace OpenMod.API.Prioritization
{
    /// <summary>
    /// Sets the priority for the given implementation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class PriorityAttribute : Attribute
    {
        public Priority Priority { get; set; } = Priority.Normal;
    }
}