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
            IOpenModComponent ownerComponent,
            EventCallback callback,
            EventListenerAttribute attribute,
            string eventName)
        {
            Owner = new WeakReference(ownerComponent);
            Callback = callback;
            EventListenerAttribute = attribute;
            EventName = eventName;
        }

        public EventSubscription(
            IOpenModComponent ownerComponent,
            IEventListener listener,
            MethodInfo method,
            EventListenerAttribute attribute, 
            Type eventType)
        {
            Owner = new WeakReference(ownerComponent);
            Listener = listener;
            Callback = (sender, @event) => method.InvokeWithTaskSupportAsync(listener, new[] { sender, @event });
            EventListenerAttribute = attribute;
            EventName = eventType.Name;
            EventType = eventType;
        }

        public EventSubscription(
            IOpenModComponent ownerComponent,
            EventCallback callback,
            EventListenerAttribute attribute, 
            Type eventType)
        {
            Owner = new WeakReference(ownerComponent);
            Callback = callback;
            EventListenerAttribute = attribute;
            EventName = eventType.Name;
            EventType = eventType;
        }

        public string EventName { get; }
     
        [CanBeNull]
        public Type EventType { get; set; }

        public WeakReference Owner { get; set; }

        public EventCallback Callback { get; }

        public EventListenerAttribute EventListenerAttribute { get; }

        [CanBeNull]
        public IEventListener Listener { get; }
    }
}