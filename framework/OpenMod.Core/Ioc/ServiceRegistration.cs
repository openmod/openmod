using System;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Prioritization;

namespace OpenMod.Core.Ioc
{
    public class ServiceRegistration
    {
        public Priority Priority { get; set; }

        public Type ServiceImplementationType { get; set; }

        public Type[] ServiceTypes { get; set; }

        public ServiceLifetime Lifetime { get; set; }
    }
}