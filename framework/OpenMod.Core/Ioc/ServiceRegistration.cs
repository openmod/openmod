using System;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API;
using OpenMod.API.Prioritization;

namespace OpenMod.Core.Ioc
{
    [OpenModInternal]
    public class ServiceRegistration
    {
        public Priority Priority { get; set; }

        public Type ServiceImplementationType { get; set; }

        public Type[] ServiceTypes { get; set; }

        public ServiceLifetime Lifetime { get; set; }
    }
}