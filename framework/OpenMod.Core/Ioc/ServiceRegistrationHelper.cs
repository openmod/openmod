using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using OpenMod.API;
using OpenMod.API.Ioc;
using OpenMod.Common.Helpers;

namespace OpenMod.Core.Ioc
{
    [OpenModInternal]
    public static class ServiceRegistrationHelper
    {
        // This method doesn't warn about missing dependencies
        // At normal om running it already notified user
        public static IEnumerable<ServiceRegistration> FindFromAssembly<T>(Assembly assembly, ILogger? logger = null) where T : ServiceImplementationAttribute
        {
            var types = assembly.GetLoadableTypes();
            return types.Where(t => t.IsClass && !t.IsAbstract).FindServicesFromTypes<T>();
        }

        public static IEnumerable<ServiceRegistration> FindServicesFromTypes<T>(this IEnumerable<Type> types, ILogger? logger = null, string? assemblyName = null) where T : ServiceImplementationAttribute
        {
            foreach (var type in types)
            {
                T? attribute;
                Type[] interfaces;

                try
                {
                    attribute = type.GetCustomAttribute<T>(inherit: false);
                    
                    if (attribute == null)
                    {
                        continue;
                    }

                    interfaces = type.GetInterfaces()
                        .Where(d => d.GetCustomAttribute<ServiceAttribute>() != null)
                        .ToArray();

                    if (interfaces.Length == 0)
                    {
                        logger?.LogWarning(
                            "Type {TypeName} in assembly {AssemblyName} has been marked as ServiceImplementation but does not inherit any services!\nDid you forget to add [Service] to your interfaces?",
                            type.FullName, assemblyName ?? type.Assembly.FullName);
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    logger?.LogWarning(ex,
                        "FindFromAssembly has failed for type: {TypeName} while searching for {SearchTypeName}",
                        type.FullName, typeof(T).FullName);
                    continue;
                }

                yield return new ServiceRegistration
                {
                    Priority = attribute.Priority,
                    ServiceImplementationType = type,
                    ServiceTypes = interfaces,
                    Lifetime = attribute.Lifetime
                };
            }
        }
    }
}