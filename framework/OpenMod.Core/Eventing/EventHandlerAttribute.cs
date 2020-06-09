using System;
using OpenMod.API.Prioritization;

namespace OpenMod.Core.Eventing
{
    [AttributeUsage(AttributeTargets.Method)]
    public class EventHandlerAttribute : Attribute
    {
        public Priority Priority { get; set; } = Priority.Normal;
        public bool IgnoreCancelled { get; set; } = false;
    }
}