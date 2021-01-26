using System;
using JetBrains.Annotations;
using OpenMod.API.Eventing;

namespace OpenMod.Core.Eventing
{
    [MeansImplicitUse]
    [AttributeUsage(AttributeTargets.Method)]
    public class EventListenerAttribute : Attribute
    {
        public EventListenerPriority Priority { get; set; } = EventListenerPriority.Normal;

        public bool IgnoreCancelled { get; set; } = false;
    }
}