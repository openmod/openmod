using JetBrains.Annotations;
using OpenMod.API.Eventing;
using System;

namespace OpenMod.Core.Eventing
{
    [MeansImplicitUse]
    [AttributeUsage(AttributeTargets.Method)]
    public class EventListenerAttribute : Attribute, IEventListenerOptions
    {
        public EventListenerPriority Priority { get; set; } = EventListenerPriority.Normal;

        public bool IgnoreCancelled { get; set; } = false;
    }
}