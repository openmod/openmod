using System;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Prioritization;

namespace OpenMod.API.Ioc
{
    [MeansImplicitUse]
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ServiceImplementationAttribute : PriorityAttribute
    {
        public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Transient;
    }
}