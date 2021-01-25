using System;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Prioritization;

namespace OpenMod.API.Ioc
{
    /// <summary>
    /// Declares a service implementation for automatic global IoC registratrion.
    /// Service implementations using this attribute are automatically registered for any interface that implements the <see cref="ServiceAttribute"/>.
    /// <seealso cref="ServiceAttribute"/>
    /// <seealso cref="PluginServiceImplementationAttribute"/>
    /// </summary>
    [MeansImplicitUse]
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ServiceImplementationAttribute : PriorityAttribute
    {
        public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Transient;
    }
}