using System;
using OpenMod.API.Prioritization;

namespace OpenMod.Core.Eventing
{
    [AttributeUsage(AttributeTargets.Method)]
    public class EventListenerAttribute : PriorityAttribute
    {
        public bool IgnoreCancelled { get; set; } = false;
    }
}