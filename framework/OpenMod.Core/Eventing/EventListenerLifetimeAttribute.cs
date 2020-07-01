using System;
using Microsoft.Extensions.DependencyInjection;

namespace OpenMod.Core.Eventing
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EventListenerLifetimeAttribute : Attribute
    {
        public ServiceLifetime Lifetime { get; }

        public EventListenerLifetimeAttribute(ServiceLifetime lifetime)
        {
            Lifetime = lifetime;
        }
    }
}