using System;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Helpers.Prioritization;

namespace OpenMod.API.Ioc
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class ServiceImplementationAttribute : Attribute
    {
        public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Transient;

        public Priority Priority { get; set; } = Priority.High;
    }
}