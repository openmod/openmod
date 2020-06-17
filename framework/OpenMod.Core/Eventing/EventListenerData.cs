using System;
using System.Reflection;

namespace OpenMod.Core.Eventing
{
    public readonly struct EventListenerData
    {
        public readonly Type EventListenerType;
        public readonly MethodInfo Method;
        public readonly EventListenerAttribute EventListenerAttribute;
        public readonly Type EventType;

        public EventListenerData(Type eventListenerType, MethodInfo method, EventListenerAttribute eventListenerAttribute, Type eventType)
        {
            EventListenerType = eventListenerType;
            Method = method;
            EventListenerAttribute = eventListenerAttribute;
            EventType = eventType;
        }
    }
}