using System;
using JetBrains.Annotations;
using OpenMod.API.Prioritization;

namespace OpenMod.Core.Eventing
{
    [MeansImplicitUse]
    [AttributeUsage(AttributeTargets.Method)]
    public class EventListenerAttribute : PriorityAttribute
    {
        public bool IgnoreCancelled { get; set; } = false;
    }
}