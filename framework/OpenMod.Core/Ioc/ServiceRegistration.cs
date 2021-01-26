using System;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API;
using OpenMod.API.Prioritization;

namespace OpenMod.Core.Ioc
{
    [OpenModInternal]
    public class ServiceRegistration
    {
        public Priority Priority { get; set; } = Priority.Normal;

        public Type ServiceImplementationType { get; set; } = null!;

        public Type[] ServiceTypes { get; set; } = null!;

        public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Transient;
    }
}