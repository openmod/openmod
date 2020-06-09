using System;
using System.Reflection;
using JetBrains.Annotations;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.Core.Helpers;

namespace OpenMod.Core.Eventing
{
    internal class EventSubscription
    {
        public EventSubscription(
            IOpenModComponent owner,
            EventCallback callback,
            EventHandlerAttribute attribute,
            string eventName)
        {
            Owner = new WeakReference(owner);
            Callback = callback;
            EventHandlerAttribute = attribute;
            EventName = eventName;
        }

        public EventSubscription(
            IOpenModComponent owner,
            IEventListener listener,
            MethodInfo method,
            EventHandlerAttribute attribute, 
            Type eventType)
        {
            Owner = new WeakReference(owner);
            Listener = listener;
            Callback = (sender, @event) => method.InvokeWithTaskSupport(listener, new[] { sender, @event });
            EventHandlerAttribute = attribute;
            EventName = eventType.Name;
            EventType = eventType;
        }

        public EventSubscription(
            IOpenModComponent owner,
            EventCallback callback,
            EventHandlerAttribute attribute, 
            Type eventType)
        {
            Owner = new WeakReference(owner);
            Callback = callback;
            EventHandlerAttribute = attribute;
            EventName = eventType.Name;
            EventType = eventType;
        }

        public string EventName { get; }
     
        [CanBeNull]
        public Type EventType { get; set; }

        public WeakReference Owner { get; set; }

        public EventCallback Callback { get; }

        public EventHandlerAttribute EventHandlerAttribute { get; }

        [CanBeNull]
        public IEventListener Listener { get; }
    }
}