using System;
using JetBrains.Annotations;
using OpenMod.API.Eventing;
using OpenMod.API.Prioritization;

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